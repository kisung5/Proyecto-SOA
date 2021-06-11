#!/bin/bash

mongodump --forceTableScan --uri 'mongodb://root:example@127.0.0.1:27017/document_analyzer?authSource=admin' -o ./mongodb_data --gzip