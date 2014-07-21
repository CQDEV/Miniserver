(function () {
    window.minidata = new function () {
        var self = this;

        self.query = function (collection, callback) {
            $.ajax({
                url: '/$/' + collection,
                success: callback
            });
        };

        self.read = function (collection, id, callback) {
            $.ajax({
                url: '/$/' + collection + '/' + id,
                success: callback
            });
        };

        self.create = function (collection, data, callback) {
            $.ajax({
                url: '/$/' + collection,
                data: JSON.stringify(data),
                method: 'post',
                success: callback
            });
        };

        self.update = function (collection, id, data, callback) {
            $.ajax({
                url: '/$/' + collection + '/' + id,
                data: JSON.stringify(data),
                method: 'put',
                success: callback
            });
        };

        self.delete = function (collection, id, callback) {
            $.ajax({
                url: '/$/' + collection + '/' + id,
                method: 'delete',
                success: callback
            });
        };
    };
})();