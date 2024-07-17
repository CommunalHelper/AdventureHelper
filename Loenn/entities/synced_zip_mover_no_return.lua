local zipMoverHelper = require("mods").requireFromPlugin("libraries.zipMoverHelper")

local syncedZipMoverNoReturn = {}

syncedZipMoverNoReturn.name = "AdventureHelper/LinkedZipMoverNoReturn"
syncedZipMoverNoReturn.depth = -9999
syncedZipMoverNoReturn.nodeVisibility = "never"
syncedZipMoverNoReturn.nodeLimits = {1, 1}
syncedZipMoverNoReturn.minimumSize = {16, 16}

syncedZipMoverNoReturn.fieldInformation = {
    colorCode = {
        fieldType = "color"
    },
    spritePath = {
        editable = true,
        options = { "", "objects/zipmover" }
    },
    speedMultiplier = {
        minimumValue = 0.0
    }
}

syncedZipMoverNoReturn.placements = {
    {
        name = "normal",
        data = {
            width = 16,
            height = 16,
            colorCode = "ffffff",
            spritePath = ""
        }
    },
    {
        name = "custom_speed",
        data = {
            width = 16,
            height = 16,
            colorCode = "ffffff",
            spritePath = "",
            speedMultiplier = 1.0
        }
    }
}

local options = {
    defaultCogTexture = "objects/AdventureHelper/noreturnzipmover/cog",
    defaultLightsTexture = "objects/zipmover/light01",
    defaultBlockTexture = "objects/AdventureHelper/noreturnzipmover/block",
}

syncedZipMoverNoReturn.sprite = zipMoverHelper.getSpriteFunction(options)
syncedZipMoverNoReturn.selection = zipMoverHelper.getSelectionFunction(options)

return syncedZipMoverNoReturn
