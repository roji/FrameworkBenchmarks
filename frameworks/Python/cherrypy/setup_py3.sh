#!/bin/bash

fw_depends python3

$PY3_ROOT/bin/pip install --install-option="--prefix=${PY3_ROOT}" -r $TROOT/requirements.txt

$PY3_ROOT/bin/python3 app.py &