#!/bin/bash

ln -s /etc/ssl/certs /usr/local/ssl/certs
/usr/libexec/s2i/assemble
rc=$?

exit $rc