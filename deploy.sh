#!/bin/bash
# Run this on the BACKEND EC2 instance
# Usage: sudo bash deploy.sh

set -e

FRONTEND_EC2_IP="<FRONTEND_EC2_PUBLIC_IP>"   # <-- replace
DOTNET_ROOT="/home/ubuntu/.dotnet"
DOTNET="$DOTNET_ROOT/dotnet"

echo "=== Installing .NET 10 ==="
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
sudo -u ubuntu ./dotnet-install.sh --channel 10.0 --install-dir $DOTNET_ROOT

# Symlink dotnet to /usr/local/bin so systemd can find it
ln -sf $DOTNET /usr/local/bin/dotnet

echo "=== Building the app ==="
cd /home/ubuntu/legacy_backend
$DOTNET publish -c Release -o ./publish

echo "=== Updating CORS allowed origin ==="
sed -i "s|http://localhost:4200|http://${FRONTEND_EC2_IP}|g" publish/appsettings.json

echo "=== Installing systemd service ==="
cat > /etc/systemd/system/blogapi.service <<EOF
[Unit]
Description=Blog API - ASP.NET Core
After=network.target

[Service]
WorkingDirectory=/home/ubuntu/legacy_backend/publish
ExecStart=/usr/local/bin/dotnet /home/ubuntu/legacy_backend/publish/BlogApi.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=blogapi
User=ubuntu
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=DOTNET_ROOT=/home/ubuntu/.dotnet

[Install]
WantedBy=multi-user.target
EOF

systemctl daemon-reload
systemctl enable blogapi
systemctl restart blogapi

echo "=== Opening port 5000 in firewall ==="
ufw allow 5000/tcp || true

echo "=== Done! Backend running on http://0.0.0.0:5000 ==="
systemctl status blogapi --no-pager
