(function () {
    var searchbox = $('#search'),
        autocomplete = $('#search-autocomplete'),
        loadMoreButton = $('.load-more', autocomplete),
        spinner = $('.loader', autocomplete),
        noResults = $('.no-results', autocomplete),
        error = $('.error', autocomplete);

    var currentOffset = 0,
        searchUrl = 'http://www.corsproxy.com/ajax.googleapis.com/ajax/services/search/web',
        lastSearch = '',
        searchTimeout = 0.5, //seconds
        searchTimer;

    function loadData() {
        noResults.hide();
        error.hide();
        loadMoreButton.hide();
        spinner.show();

        if (searchbox.val() !== lastSearch) {
            $('.search-result', autocomplete).remove();
            currentOffset = 0;
        }

        lastSearch = searchbox.val();

        $.ajax({
            type: 'GET',
            url: searchUrl,
            dataType: 'text',
            data: {
                'v': '1.0',
                'q': lastSearch,
                'start': currentOffset
            },
            success: function (data) {
                var json = $.parseJSON(data);

                if (json &&
                   json.responseData &&
                   json.responseData.results &&
                   json.responseData.results.length > 0) {
                    for (var i = 0; i < json.responseData.results.length; ++i) {
                        $('<li class="search-result">' +
                            '<a href="' + json.responseData.results[i].url + '" target="_blank">' +
                              json.responseData.results[i].titleNoFormatting +
                              '<span class="search-description">' +
                                json.responseData.results[i].content +
                              '</span>' +
                            '</a>' +
                          '</li>').insertBefore(loadMoreButton);
                    }

                    noResults.hide();
                    loadMoreButton.show();
                    error.hide();
                } else {
                    noResults.show();
                    loadMoreButton.hide();
                    error.hide();
                }

                spinner.hide();
                currentOffset += json.responseData.results.length;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error(errorThrown);

                error.text(error.text().replace('%err%', textStatus + ': ' + errorThrown))
                     .show();

                loadMoreButton.hide();
                noResults.hide();
                spinner.hide();
            }
        });
    }

    searchbox.on('focus', function () {
        if ($(this).val().length > 0) {
            autocomplete.slideDown('fast');
        }
    })
    .on('input propertychange paste', function () {
        //this will handle pasting text and clearing text with browser built in clear button
        $(this).trigger('keyup');
    })
    .on('keydown', function () {
        clearTimeout(searchTimer);
    })
    .on('keyup', function () {
        if ($(this).val().length > 0) {
            clearTimeout(searchTimer);
            searchTimer = setTimeout(loadData, searchTimeout * 1000);
            autocomplete.slideDown('fast');
        } else {
            clearTimeout(searchTimer);
            autocomplete.slideUp('fast');
        }
    });

    $(document).on('click', function (e) {
        //click anywhere outside searchbox to close
        if (!$(e.target).closest('.searchbox').length) {
            if (autocomplete.is(':visible')) {
                autocomplete.slideUp('fast');
                clearTimeout(searchTimer);
            }
        }
    });


    autocomplete.css('top', (searchbox.outerHeight()) + 'px');
    loadMoreButton.on('click', loadData)
                  .hide();
    noResults.hide();
    error.hide();

}());