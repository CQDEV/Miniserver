(function () {
    minidata.create('testCollection', { key: 'a', value: 1 });

    minidata.query('testCollection');

    minidata.read('testCollection', '1');
    
    minidata.update('testCollection', '1', { key: 'a', value: 100 });

    minidata.delete('testCollection', '1');
})();