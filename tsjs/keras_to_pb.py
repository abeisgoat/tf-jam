#!/usr/bin/env python3
from keras.models import load_model
import tensorflow as tf
import sys

sess = tf.Session()

from keras import backend as K
K.set_session(sess)
model = load_model('model_as_keras/model.h5')

def freeze_session(session, keep_var_names=None, output_names=None, clear_devices=True):
    """
    Freezes the state of a session into a pruned computation graph.

    Creates a new computation graph where variable nodes are replaced by
    constants taking their current value in the session. The new graph will be
    pruned so subgraphs that are not necessary to compute the requested
    outputs are removed.
    @param session The TensorFlow session to be frozen.
    @param keep_var_names A list of variable names that should not be frozen,
                          or None to freeze all the variables in the graph.
    @param output_names Names of the relevant graph outputs.
    @param clear_devices Remove the device directives from the graph for better portability.
    @return The frozen graph definition.
    """
    from tensorflow.python.framework.graph_util import convert_variables_to_constants
    graph = session.graph
    with graph.as_default():
        freeze_var_names = list(set(v.op.name for v in tf.global_variables()).difference(keep_var_names or []))
        output_names = output_names or []
        output_names += [v.op.name for v in tf.global_variables()]
        input_graph_def = graph.as_graph_def()
        if clear_devices:
            for node in input_graph_def.node:
                node.device = ""
        frozen_graph = convert_variables_to_constants(session, input_graph_def,
                                                      output_names, freeze_var_names)
        return frozen_graph


tSaver = tf.train.Saver()
tSaver.save(sess, "model_as_checkpoint/Profile.ckpt")

print([n.name for n in tf.get_default_graph().as_graph_def().node])
print(model.summary())

#frozen_graph = freeze_session(sess)
tf.train.write_graph(sess.graph_def, "./model_as_pb", "model.pb", as_text=True)