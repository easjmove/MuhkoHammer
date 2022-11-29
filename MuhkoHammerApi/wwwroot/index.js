var app = new Vue({
    el: '#app',
    data: {
        baseurl: "/api/",
        gameSystems: [{ Name: "test", Id: 1 }],
        gameSystemID: -1,
        factions: [],
        factionID: -1,
        units: [],
        unitID: -1,
        includeAll: false
    },
    created: function () {
        this.includeAll = document.getElementById("HiddenIncludeAll").value;
        this.getGameSystems(this.includeAll);
    },
    methods: {
        getGameSystems() {
            axios.get(this.baseurl + 'GameSystems?includeAll=' + this.includeAll)
                .then((response) => {
                    this.gameSystems = response.data;
                });
        },
        gameSystemChange() {
            this.factionID = -1;
            this.unitID = -1;
            if (this.gameSystemID == "-1") {
                this.factions.splice(0);
                this.units.splice(0);
                this.unitImages.splice(0);
            } else {
                axios.get(this.baseurl + 'GameSystems/' + this.gameSystemID + "/Factions?includeAll=" + this.includeAll)
                    .then((response) => {
                        this.factions = response.data;
                    });
            }
        },
        factionChange() {
            this.unitID = -1;
            if (this.factionID == "-1") {
                this.units.splice(0);
                this.unitImages.splice(0);
            } else {
                axios.get(this.baseurl + 'GameSystems/' + this.gameSystemID + "/Factions/" + this.factionID + "/Units?includeAll=" + this.includeAll)
                    .then((response) => {
                        this.units = response.data;
                    });
            }
        },
        unitChange() {
            if (this.unitID == "-1") {
                this.unitImages == [];
            } else {
                axios.get(this.baseurl + 'GameSystems/' + this.gameSystemID + "/Factions/" + this.factionID + "/Units/" + this.unitID + "/Images?includeAll=" + this.includeAll)
                    .then(response => {
                        $('.carousel-item').each(function (i, obj) {
                            obj.remove();
                        });

                        var isFirst = true;
                        $.each(response.data, function (key, data) {
                            if (isFirst) {
                                $('#imageCarousel').append($('<div class="carousel-item item active"><img class="d-block img-fluid myImg" src=' + data.url + '></div>'));
                            } else {
                                $('#imageCarousel').append($('<div class="carousel-item item"><img class="d-block img-fluid myImg" src=' + data.url + '></div>'));
                            }
                            isFirst = false;
                        });

                        if (isFirst) {
                            $('#imageCarousel').append($('<div class="carousel-item item active"><img class="d-block img-fluid myImg" src="/images/warhammer-40k.jpg"></div>'));
                            $('#imageCarousel').append($('<div class="carousel-item item"><img class="d-block img-fluid myImg" src="/images/soul.jpg"></div>'));
                        }

                        $('#imageCarousel').carousel();
                    });
            }
        },
        unitChange2(includeAll) {
            if (this.unitID == "-1") {
                this.unitImages == [];
            } else {
                axios.get(this.baseurl + 'GameSystems/' + this.gameSystemID + "/Factions/" + this.factionID + "/Units/" + this.unitID + "/Images?includeAll=" + this.includeAll)
                    .then(response => {
                        $('#images').empty();

                        $.each(response.data, function (key, data) {
                            $('#images').append('<a href="' + data.url + '" target="_blank"><img src="' + data.thumbUrl + '" width="150" height="150" /></a>');
                        });
                    });
            }
        },
        uploadImage(event) {
            var currentUnitID = this.unitID;
            if (!currentUnitID) {
                currentUnitID = -1;
            }

            var unitUrl = this.baseurl + "Images/?unitID=" + currentUnitID;

            $.each(event.target.files, function (key, data) {
                //fileObject = event.target.files[0];

                var formData = new FormData();
                formData.append("data", data);

                axios.post(unitUrl, formData)
                    .then(
                        response => {
                            console.log('image upload response > ', response)
                            alert("Image uploaded :)");
                        }
                    )
            });
        }
    }
})