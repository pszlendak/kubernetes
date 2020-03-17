# Create Cockroach cluster pods
```
kubectl create -f cockroachdb-statefulset.yaml
```
# Join the cluster
```
kubectl create -f cluster-init-secure.yaml
```
