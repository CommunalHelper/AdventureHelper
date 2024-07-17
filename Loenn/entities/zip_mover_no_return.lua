local zipMoverHelper = require("mods").requireFromPlugin("libraries.zipMoverHelper")

local zipMoverNoReturn = {}

zipMoverNoReturn.name = "AdventureHelper/ZipMoverNoReturn"
zipMoverNoReturn.depth = -9999
zipMoverNoReturn.nodeVisibility = "never"
zipMoverNoReturn.nodeLimits = {1, 1}
zipMoverNoReturn.minimumSize = {16, 16}

zipMoverNoReturn.fieldInformation = {
    spritePath = {
        editable = true,
        options = { "", "objects/zipmover" }
    },
    speedMultiplier = {
        minimumValue = 0.0
    }
}

zipMoverNoReturn.placements = {
    {
        name = "normal",
        data = {
            width = 16,
            height = 16,
            spritePath = ""
        }
    },
    {
        name = "custom_speed",
        data = {
            width = 16,
            height = 16,
            spritePath = "",
            speedMultiplier = 1.0
        }
    }
}

local options = {
    defaultCogTexture = "objects/AdventureHelper/noreturnzipmover/cog",
    defaultLightsTexture = "objects/zipmover/light01",
    defaultBlockTexture = "objects/AdventureHelper/noreturnzipmover/block",
    defaultRopeColor = {209 / 255, 209 / 255, 209 / 255},
}

zipMoverNoReturn.sprite = zipMoverHelper.getSpriteFunction(options)
zipMoverNoReturn.selection = zipMoverHelper.getSelectionFunction(options)

return zipMoverNoReturn
