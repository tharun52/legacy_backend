#!/bin/bash
# Run this on the BACKEND EC2 instance
# Usage: sudo bash deploy.sh

set -e

FRONTEND_EC2_IP="<FRONTEND_EC2_PUBLIC_IP>"   # <-- replace
UBUNTU_HOME="/home/ubuntu"
DOTNET_ROOT="$UBUNTU_HOME/.dotnet"
DOTNET="$DOTNET_ROOT/dotnet"
APP_DIR="$UBUNTU_HOME/legacy_backend"

echo "=== Installing .NET 10 ==="
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
sudo -u ubuntu bash dotnet-install.sh --channel 10.0 --install-dir $DOTNET_ROOT

# Symlink dotnet to /usr/local/bin so systemd can find it
ln -sf $DOTNET /usr/local/bin/dotnet

echo "=== Verifying dotnet ==="
$DOTNET --version

echo "=== Building the app ==="
cd $APP_DIR
$DOTNET publish -c Release -o $APP_DIR/publish

echo "=== Verifying publish output ==="
ls $APP_DIR/publish/BlogApi.dll

echo "=== Updating CORS allowed origin ==="
sed -i "s|http://localhost:4200|http://${FRONTEND_EC2_IP}|g" $APP_DIR/publish/appsettings.json

echo "=== Installing systemd service ==="
cat > /etc/systemd/system/blogapi.service <<EOF
[Unit]
Description=Blog API - ASP.NET Core
After=network.target

[Service]
WorkingDirectory=$APP_DIR/publish
ExecStart=/usr/local/bin/dotnet $APP_DIR/publish/BlogApi.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=blogapi
User=ubuntu
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000
Environment=DOTNET_ROOT=$DOTNET_ROOT

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
