# Create MySQL deployment 
1. Ensure you have a storage class with PVs created (see yamls/local-provisioner)
2. Create PVC
```
kubectl create -f mysql-pvc.yaml
```

2. Create deployment
```
kubectl create -f mysql-deployment.yaml
```
3. Connect to mysql instance
```
kubectl run -it --rm --image=mysql:8.0.19 --restart=Never mysql-client -- mysql -h mysql -ppassword
```

