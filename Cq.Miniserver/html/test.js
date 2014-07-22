(function () {
    var collectionName = 'testCollection';

    minidata.create(collectionName, { key: 'a', value: 1, date: new Date() }, function (a, b, c) {
        var id = a.id;

        debugger;

        minidata.query(collectionName, function (a, b, c) {
            debugger;

            minidata.read(collectionName, id, function (a, b, c) {
                debugger;

                minidata.update(collectionName, id, { key: 'abc', value: 123, date: new Date() }, function (a, b, c) {
                    debugger;

                    minidata.read(collectionName, id, function (a, b, c) {
                        debugger;

                        minidata.delete(collectionName, id, function (a, b, c) {
                            debugger;
                        });
                    });
                });
            });
        });
    });
})();