(function () {



    $('input.datetimepicker').datetimepicker({
        'format': 'YYYY-MM-DD',
        'minDate': moment().format('YYYY-MM-DD')
    });



    var options = {
        'format': 'HH:mm',
        'stepping': 5
    };

    var $start = $('.timepicker.start')
        .datetimepicker(options)
        .on('dp.change', function (e) {

            if (e.date) {
                $end.data("DateTimePicker").minDate(e.date.add(10, 'm'));
                $end.data("DateTimePicker").date(e.date);
            }
        });

    var $end = $('.timepicker.end')
        .datetimepicker($.extend({ useCurrent: false }, options))
        .on('dp.change', function (e) {
        });

    $('.stepping').on('change', function () {
        var $this = $(this),
            step = $this.attr('step') * 1,
            val = $this.val() * 1;

        var duration = Math.round(val / step) * step;

        $this.val(duration);

        $start.data("DateTimePicker").stepping(duration).clear();
        $end.data("DateTimePicker").stepping(duration).clear();
    });
})();

(function () {

    var $ddlFacility = $('.dynamic-facility');

    if ($ddlFacility.length) {

        var $target = $($ddlFacility.data('load-into'));
        var defaultOption = $target.children().first();
        var defaultDoctorOption = defaultOption.clone().text("Please select doctor");

        $('.dynamic-facility').on('change', function () {
            var $this = $(this);
            var $target = $($this.data('load-into'));
            var facilityId = $this.val();
            var loadingOption = defaultOption.clone().text("Loading...");

            if (!facilityId) {
                $target.html(defaultOption);
                return;
            }

            $target.html(loadingOption);
            $.ajax({
                url: '/Facilities/GetDoctors/' + facilityId,
                contentType: 'application/json; charset=utf-8',
                method: 'GET'
            }).success(function (data) {

                if (data) {

                    var optionHtml = data.map(function (item) {
                        return item ? '<option value="' + item.Id + '">' + item.Name + '</option>' : '';
                    });

                    $target.html(defaultDoctorOption);
                    $target.append(optionHtml);
                }
                else {
                    $target.html(defaultOption);
                }
            })
            .error(function (xhr, statusCode, statusText) {
                alert('Something went wrong');
            });
        });

        var $ddlDoctorService = $('.ddl-doctor-service');
        if ($ddlDoctorService.length) {
            $('.facility-doctors').on('change', function () {
                var $this = $(this);

                if ($this.val()) {
                    var value = JSON.parse($this.val());

                    $ddlDoctorService
                        .find('option').prop('disabled', true)
                        .filter('option[data-doctor-id=' + value + ']').prop('disabled', false);
                }

            });
        }

    }
})();

(function () {

    var $schedulePicker = $('.inline-datepicker').datetimepicker({
        'inline': true,
        'sideBySide': true,
        'format': 'YYYY-MM-DD',
        'minDate': moment().format('YYYY-MM-DD')
    });

    $('.datetimepicker-nolimit').datetimepicker({
        'format': 'YYYY-MM-DD'
    });

    $('.dynamic-facility.visit-facility').on('change', function () {
        ResetSelections();
    });

    var $scheduleId = $('#schedule-id');
    var $startAt = $('#start-at');
    var $endAt = $('#end-at');

    var $hourSelection = $('.hour-selection');
    var $ddlHour = $('.ddl-hour');
    var $ddlFacility = $('.dynamic-facility');
    var $ddlDoctor = $('.facility-doctors.visit-doctor').on('change', function () {
        var $this = $(this);
        var facilityId = $ddlFacility.val();
        var doctorId = $this.val();

        ResetSelections();

        $.post({
            url: '/Visits/Schedule',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify({
                doctorId: doctorId,
                facilityId: facilityId
            })
        }).done(function (data) {
            var dates = data.dates;

            $schedulePicker.data("DateTimePicker").enabledDates(dates);
            $schedulePicker.data("DateTimePicker").clear();
        })
        .fail(function () {
            alert('An error occured');
        });
    });

    $schedulePicker.on("dp.change", function (e) {

        if (!e.date) {
            return;
        }

        var facilityId = $ddlFacility.val();
        var doctorId = $ddlDoctor.val();
        ResetSelections(true);

        var date = e.date.format('YYYY-MM-DD');

        $hourSelection.addClass('hidden');

        $.post({
            url: '/Visits/Slots/',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify({
                date: date,
                doctorId: doctorId,
                facilityId: facilityId
            })
        })
        .done(function (data) {
            $hourSelection.removeClass('hidden');
            var slots = data.slots;

            var options = ['<option value="">Please select a time period</option>'];
            for (var i = 0, l = slots.length; i < l; i++) {
                var slot = slots[i];
                var doctorService = slot.DoctorService ? ' (' + slot.DoctorService + ')' : '';

                options.push("<option value='" + JSON.stringify(slot) + "'>" + slot.StartAt + " - " + slot.EndAt + doctorService + "</option>");
            }

            $ddlHour.html(options.join(''));
        })
        .fail(function () {
            alert('An error occured');
        });
    });

    $ddlHour.on('change', function () {
        var $this = $(this);

        if (!$this.val())
            return;

        var object = JSON.parse($this.val());

        $scheduleId.val(object.DoctorScheduleId);
        $startAt.val(object.StartAt);
        $endAt.val(object.EndAt);
    });

    var ResetSelections = function (dontClearDatepicker) {

        if (!dontClearDatepicker) {
            $schedulePicker.data("DateTimePicker").clear();
        }

        $hourSelection.addClass('hidden');
        $scheduleId.val('');
        $startAt.val('');
        $endAt.val('');
    };

})();

(function () {
    $('.btn-remote').on('click', function () {
        var $this = $(this),
            action = $this.data('url'),
            method = $this.data('method'),
            defaultText = $this.html(),
            behaviour = $this.data('behaviour');

        $this.prop('disabled', true);
        $this.text($this.data('loadingText'));

        $.ajax({
            url: action,
            contentType: 'application/json',
            method: method
        }).done(function (data) {

            if (data.status === true) {
                if (behaviour === 'reload') {
                    location.reload();
                }
            }
            else {
                if (data.message) {
                    alert(data.message);
                }

            }

        }).fail(function () {
            alert('An error occured please try again later');
        }).complete(function () {
            $this.html(defaultText);
            $this.prop('disabled', false);
        });

    });

    $('.frm-map').on('submit', function (e) {

        e.preventDefault();

        var $this = $(this);
        var action = $this.attr('action');
        var formData = $this.serializeArray();

        var formObject = formData.reduce(function (doctorFacility, foreignDoctorServiceId) {
            var tmpObject = {};
            tmpObject[doctorFacility.name] = JSON.parse(doctorFacility.value);
            tmpObject[foreignDoctorServiceId.name] = foreignDoctorServiceId.value;
            return tmpObject;
        });

        if (formObject) {

            $.post({
                url: action,
                data: JSON.stringify(formObject),
                contentType: 'application/json; charset=utf-8'
            }).done(function (data) {
                if (data.status === true) {
                    location.reload();
                }
                else {
                    alert('An error occured please try again later');
                }
            })
            .fail(function () {
                alert('An error occured please try again later');
            });

        }
        else {
            alert('Form is empty please check the form');
        }
    });
})();