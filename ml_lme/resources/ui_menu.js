﻿// Add settings
var g_modSettingsLME = [];

engine.on('updateModSettingLME', function (_name, _value) {
    for (var i = 0; i < g_modSettingsLME.length; i++) {
        if (g_modSettingsLME[i].name == _name) {
            g_modSettingsLME[i].updateValue(_value);
            break;
        }
    }
});

// Add own menu
{
    let l_block = document.createElement('div');
    l_block.innerHTML = `
        <div class ="settings-subcategory">
            <div class ="subcategory-name">Leap Motion tracking</div>
            <div class ="subcategory-description"></div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Enable tracking: </div>
            <div class ="option-input">
                <div id="Enabled" class ="inp_toggle no-scroll" data-current="false"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Tracking mode: </div>
            <div class ="option-input">
                <div id="Mode" class ="inp_dropdown no-scroll" data-options="0:Screentop,1:Desktop,2:HMD" data-current="1"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Desktop offset X: </div>
            <div class ="option-input">
                <div id="DesktopX" class ="inp_slider no-scroll" data-min="-100" data-max="100" data-current="0"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Desktop offset Y: </div>
            <div class ="option-input">
                <div id="DesktopY" class ="inp_slider no-scroll" data-min="-100" data-max="100" data-current="-45"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Desktop offset Z: </div>
            <div class ="option-input">
                <div id="DesktopZ" class ="inp_slider no-scroll" data-min="-100" data-max="100" data-current="30"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Attach to head: </div>
            <div class ="option-input">
                <div id="Head" class ="inp_toggle no-scroll" data-current="false"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Head offset X: </div>
            <div class ="option-input">
                <div id="HeadX" class ="inp_slider no-scroll" data-min="-100" data-max="100" data-current="0"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Head offset Y: </div>
            <div class ="option-input">
                <div id="HeadY" class ="inp_slider no-scroll" data-min="-100" data-max="100" data-current="-30"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Head offset Z: </div>
            <div class ="option-input">
                <div id="HeadZ" class ="inp_slider no-scroll" data-min="-100" data-max="100" data-current="15"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Offset angle X: </div>
            <div class ="option-input">
                <div id="AngleX" class ="inp_slider no-scroll" data-min="-180" data-max="180" data-current="0"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Offset angle Y: </div>
            <div class ="option-input">
                <div id="AngleY" class ="inp_slider no-scroll" data-min="-180" data-max="180" data-current="0"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Offset angle Z: </div>
            <div class ="option-input">
                <div id="AngleZ" class ="inp_slider no-scroll" data-min="-180" data-max="180" data-current="0"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Track elbows: </div>
            <div class ="option-input">
                <div id="TrackElbows" class ="inp_toggle no-scroll" data-current="true"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Fingers tracking only: </div>
            <div class ="option-input">
                <div id="FingersOnly" class ="inp_toggle no-scroll" data-current="false"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Model visibility: </div>
            <div class ="option-input">
                <div id="Model" class ="inp_toggle no-scroll" data-current="false"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Visualize hands: </div>
            <div class ="option-input">
                <div id="VisualHands" class ="inp_toggle no-scroll" data-current="false"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Interaction input: </div>
            <div class ="option-input">
                <div id="Interaction" class ="inp_toggle no-scroll" data-current="false"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Recognize gestures: </div>
            <div class ="option-input">
                <div id="Gestures" class ="inp_toggle no-scroll" data-current="false"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Interact gesture threadhold: </div>
            <div class ="option-input">
                <div id="InteractThreadhold" class ="inp_slider no-scroll" data-min="0" data-max="100" data-current="80"></div>
            </div>
        </div>

        <div class ="row-wrapper">
            <div class ="option-caption">Grip gesture threadhold: </div>
            <div class ="option-input">
                <div id="GripThreadhold" class ="inp_slider no-scroll" data-min="0" data-max="100" data-current="40"></div>
            </div>
        </div>
    `;
    document.getElementById('settings-implementation').appendChild(l_block);

    // Update toggles in new menu block
    let l_toggles = l_block.querySelectorAll('.inp_toggle');
    for (var i = 0; i < l_toggles.length; i++) {
        g_modSettingsLME[g_modSettingsLME.length] = new inp_toggle_mod(l_toggles[i], 'MelonMod_LME_Call_InpToggle');
    }

    // Update sliders in new menu block
    let l_sliders = l_block.querySelectorAll('.inp_slider');
    for (var i = 0; i < l_sliders.length; i++) {
        g_modSettingsLME[g_modSettingsLME.length] = new inp_slider_mod(l_sliders[i], 'MelonMod_LME_Call_InpSlider');
    }

    //Update dropdowns in new menu block
    let l_dropdowns = l_block.querySelectorAll('.inp_dropdown');
    for (var i = 0; i < l_dropdowns.length; i++) {
        g_modSettingsLME[g_modSettingsLME.length] = new inp_dropdown_mod(l_dropdowns[i], 'MelonMod_LME_Call_InpDropdown');
    }
}
