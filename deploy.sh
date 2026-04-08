#!/bin/bash
# Run this on the BACKEND EC2 instance
# Usage: bash deploy.sh

set -e

FRONTEND_EC2_IP="<FRONTEND_EC2_PUBLIC_IP>"   # <-- replace

echo "=== Installing .NET 10 ==="
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$HOME/.dotnet

echo "=== Building the app ==="
cd /home/ubuntu/legacy_backend
dotnet publish -c Release -o ./bin/Release/net10.0

echo "=== Updating CORS allowed origin ==="
# Update appsettings.json with the actual frontend IP
sed -i "s|http://localhost:4200|http://${FRONTEND_EC2_IP}|g" appsettings.json

echo "=== Installing systemd service ==="
sudo cp blogapi.service /etc/systemd/system/blogapi.service
sudo systemctl daemon-reload
sudo systemctl enable blogapi
sudo systemctl restart blogapi

echo "=== Opening port 5000 in firewall ==="
sudo ufw allow 5000/tcp || true

echo "=== Done! Backend running on http://0.0.0.0:5000 ==="
sudo systemctl status blogapi --no-pager
