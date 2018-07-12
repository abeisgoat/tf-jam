const tf = require("@tensorflow/tfjs");
const fs = require("fs");
require('@tensorflow/tfjs-node');

(async () => {
    /*
        Load our csv file and get it into a properly shaped array of pairs likes...
        [
            [distanceA, forceB],
            [distanceB, forceB],
            ...
        ]
     */
    var pairs = getPairsFromCSV().slice(0, 1000);

    /*
        Train the model using the data.
     */
    var model = tf.sequential();
    model.add(tf.layers.dense({units: 1, inputShape: [1], name: "shots"}));
    model.compile({loss: 'meanSquaredError', optimizer: 'sgd'});

    console.log(pairs);

    const xs = tf.tensor1d(pairs.map((p) => p[0] / 100));
    const ys = tf.tensor1d(pairs.map((p) => p[1]));

    console.log(`Training ${pairs.length}...`);
    await model.fit(xs, ys, {epochs: 100});

    await model.save("file://./model_as_tsjs");

    console.log(model.inputs);
    console.log(model.outputNames)
})();

function getPairsFromCSV() {
    return fs.readFileSync("../successful_shots.csv").toString().split("\n").map((row) => {
        const pair = row.split(",").slice(1).map((field) => {
            return parseFloat(field);
        });

        return pair.length ? pair : null;
    }).filter((p) => p);
}