#!/bin/bash

restic -r test-repo backup source-files --password-file password-file
