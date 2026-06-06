#!/bin/bash
set -e
mkdir -p /jieba_dict
[ -f /jieba_dict/user_dict.txt ] || touch /jieba_dict/user_dict.txt
exec docker-entrypoint.sh "$@"
