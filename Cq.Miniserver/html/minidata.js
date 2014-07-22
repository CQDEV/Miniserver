(function () {
    window.minidata = new function () {
        var self = this;

        self.error = function (a, b, c) {
            debugger;
        };

        self.query = function (collection, callback) {
            $.ajax({
                url: '/$/' + collection,
                success: callback,
                error: self.error
            });
        };

        self.read = function (collection, id, callback) {
            $.ajax({
                url: '/$/' + collection + '/' + id,
                success: callback,
                error: self.error
            });
        };

        self.create = function (collection, data, callback) {
            $.ajax({
                url: '/$/' + collection,
                data: JSON.stringify(data),
                method: 'post',
                success: callback,
                error: self.error
            });
        };

        self.update = function (collection, id, data, callback) {
            $.ajax({
                url: '/$/' + collection + '/' + id,
                data: JSON.stringify(data),
                method: 'put',
                success: callback,
                error: self.error
            });
        };

        self.delete = function (collection, id, callback) {
            $.ajax({
                url: '/$/' + collection + '/' + id,
                method: 'delete',
                success: callback,
                error: self.error
            });
        };
    };
})();