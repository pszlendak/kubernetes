# Install microk8s
echo "[TASK 1] Install microk8s"
sudo snap install microk8s --classic --channel=1.18/stable

echo "[TASK 2] Add user to microk8s group"
sudo usermod -a -G microk8s $USER
sudo chown -f -R $USER ~/.kube

echo "[TASK 3] Set kubectl alias for microk8s kubectl"
echo "alias kubectl='microk8s kubectl'" >> ~/.bash_aliases

echo "[TASK 4] Wait till microk8s is ready ..."
microk8s status --wait-ready

echo "[DONE] microk8s is ready!"

