#!/bin/bash

ln -s /usr/local/ssl/certs /etc/ssl/certs
/usr/libexec/s2i/assemble
rc=$?

exit $rc