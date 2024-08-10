#!/bin/bash

restic -r test-repo find "*" --password-file password-file > findings
echo "Findings generated!"
