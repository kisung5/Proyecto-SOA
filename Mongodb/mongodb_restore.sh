#!/bin/bash

mongorestore --uri 'mongodb://root:example@127.0.0.1:27017/document_analyzer?authSource=admin' --dir ./mongodb_data --gzip