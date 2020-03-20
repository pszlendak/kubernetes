# Configure local volumes
1. Ensure you have some disks mounted to /mnt/fast-disks mount points on any nodes
```
[vagrant@kworker1 kworker1-ssd2]$ mount | grep fast
/dev/sdb1 on /mnt/fast-disks/kworker1-ssd1 type ext4 (rw,relatime,data=ordered)
/dev/sdb2 on /mnt/fast-disks/kworker1-ssd2 type ext4 (rw,relatime,data=ordered)
```

2. Create storage class
```
kubectl create -f fast-disks.yaml
```

3. Create persistence volumes
```
kubectl create -f provisioner_generated.yaml
```

4. Check the volmues were created
```
kubectl get pv
```

For the complete How-to go to https://github.com/kubernetes-sigs/sig-storage-local-static-provisioner/blob/master/docs/getting-started.md
