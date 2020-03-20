# Create Cockroach cluster pods
1. Make sure you have an ntp service running on every node of the kubernetes cluster and that the time has been synchronized.
CockroachDB won't run otherwise.
```
systemctl restart ntdp
```

2. Create kubernetes statefulset
```
kubectl create -f cockroachdb-statefulset.yaml
```
3. Join the cluster
```
kubectl create -f cluster-init.yaml
```

4. Create a test database
```
[vagrant@kmaster yamls]$ kubectl run cockroachdb -it --image=cockroachdb/cockroach:v19.2.4 --rm --restart=Never -- sql --insecure --host=cockroachdb-public
If you don't see a command prompt, try pressing enter.
root@cockroachdb-public:26257/defaultdb> create database test;
CREATE DATABASE

Time: 16.538644ms

root@cockroachdb-public:26257/defaultdb>
```
5. Run a test program on Linqpad (Windows)

```
lprun6 .\cockroachdb.linq
```


