freeze_graph --input_graph=./model_as_pb/model.pb --input_checkpoint=./model_as_checkpoint/Profile.ckpt --output_node_name=shots/BiasAdd --output_graph=../Assets/Resources/frozen.pb.bytes
