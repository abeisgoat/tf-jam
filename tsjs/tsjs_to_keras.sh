#!/usr/bin/env bash
tensorflowjs_converter     --input_format tensorflowjs --output_format keras ./model_as_tsjs/model.json ./model_as_keras/model.h5
