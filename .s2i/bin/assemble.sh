#!/bin/bash
echo "Add certs to right location"
ln -s /etc/ssl/certs /usr/local/ssl/certs
ls /usr/local/ssl/certs
/usr/libexec/s2i/assemble
rc=$?

exit $rc