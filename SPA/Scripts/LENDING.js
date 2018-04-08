var $jq = jQuery;

function CropImage() {
    var img = $jq('#preview-pane .preview-container img');
    $jq('#image_crop').modal('hide');
    $jq("#" + $('#crop-avatar-target').attr("DisplayId")).css({
        width: img.css('width'),
        height: img.css('height'),
        marginLeft: img.css('marginLeft'),
        marginTop: img.css('marginTop')
    });
    $jq("#Consider_" + $('#crop-avatar-target').attr("DisplayId")).val(img.css('width')+","+img.css('height')+","+img.css('marginLeft')+","+img.css('marginTop'));
}
function setCropImage(input, id) {
    if (input.files && input.files[0]) {
        
        var reader = new FileReader();
        reader.onload = function (e) {
            /*Set Image for Croping*/
            $jq('#crop-avatar-target').attr('src', e.target.result).attr("DisplayId", id);
            $jq('#preview-pane .preview-container img').attr('src', e.target.result);
            $jq('.jcrop-holder img').attr('src', e.target.result);
            $jq('#' + id).attr('src', e.target.result);
            /*Show Delete Button*/
            $jq("#" + id + "_Delete").show();
            /*Just enter any text into Hidden because it decides whether to save Image or not*/
            $jq("#Hidden_" + id).val(input.files[0]);
        }
        reader.readAsDataURL(input.files[0]);
        $jq('#avatar-crop-box').removeClass('hidden');
        $jq("#CloseandPay").modal({
            backdrop: 'static',
            keyboard: false
        });
    }
}
function updatePreviewPane(c) {
    if (parseInt(c.w) > 0) {
        var rx = xsize / c.w;
        var ry = ysize / c.h;
        $jq('#preview-pane .preview-container img').css({
            width: Math.round(rx * boundx) + 'px',
            height: Math.round(ry * boundy) + 'px',
            marginLeft: '-' + Math.round(rx * c.x) + 'px',
            marginTop: '-' + Math.round(ry * c.y) + 'px'
        });
    }
}
function initAvatarCrop(img) {
    img.Jcrop({
        onChange: updatePreviewPane,
        onSelect: updatePreviewPane,
        aspectRatio: 2,
        minSize: [316, 153]
    }, function () {
        var bounds = this.getBounds();
        boundx = bounds[0];
        boundy = bounds[1];
        jcrop_api = this;
        // Maximise initial selection around the centre of the image,
        // but leave enough space so that the boundaries are easily identified.
        var padding = 10;
        var shortEdge = (boundx < boundy ? boundx : boundy) - padding;
        var longEdge = boundx < boundy ? boundy : boundx;
        var xCoord = longEdge / 2 - shortEdge / 2;
        jcrop_api.animateTo([xCoord, padding, shortEdge, shortEdge]);
        var pcnt = $jq('#preview-pane .preview-container');
        xsize = pcnt.width();
        ysize = pcnt.height();
        $jq('#preview-pane').appendTo(jcrop_api.ui.holder);
        jcrop_api.focus();
    });
}
$jq('#CloseandPay').on('shown.bs.modal', function () {
    var img = $jq('#crop-avatar-target');
    initAvatarCrop(img);
});
