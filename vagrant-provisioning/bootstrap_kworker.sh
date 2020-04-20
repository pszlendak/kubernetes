#!/bin/bash

# Join worker nodes to the Kubernetes cluster
echo "[TASK 1] Join node to Kubernetes Cluster"
yum install -q -y sshpass >/dev/null 2>&1
sshpass -p "kubeadmin" scp -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no kmaster.example.com:/joincluster.sh /joincluster.sh 2>/dev/null
bash /joincluster.sh >/dev/null 2>&1

# Mount disk to be used for local-provisioner
echo "[TASK 2] Create a disk for local-provisioner"
parted /dev/sdb mklabel msdos > /dev/null 2>&1
parted /dev/sdb mkpart primary 0% 100% > /dev/null 2>&1
mkfs.xfs /dev/sdb1 >/dev/null 2>&1
MOUNT_POINT="/mnt/fast-disks/disk1"
mkdir -p $MOUNT_POINT
echo `blkid /dev/sdb1 | awk '{print$2}' | sed -e 's/"//g'` $MOUNT_POINT xfs   noatime,nobarrier   0   0 >> /etc/fstab
mount $MOUNT_POINT >/dev/null 2>&1

