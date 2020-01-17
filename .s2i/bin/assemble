#!/bin/bash
echo "Add certs to right location"
mkdir /usr/local/ssl
ln -s /etc/ssl/certs /usr/local/ssl/certs
/usr/libexec/s2i/assemble
rc=$?

exit $rc