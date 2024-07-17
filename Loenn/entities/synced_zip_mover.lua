local zipMoverHelper = require("mods").requireFromPlugin("libraries.zipMoverHelper")

local syncedZipMover = {}

syncedZipMover.name = "AdventureHelper/LinkedZipMover"
syncedZipMover.depth = -9999
syncedZipMover.nodeVisibility = "never"
syncedZipMover.nodeLimits = {1, 1}
syncedZipMover.minimumSize = {16, 16}

syncedZipMover.fieldInformation = {
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

syncedZipMover.placements = {
    {
        name = "normal",
        data = {
            width = 16,
            height = 16,
            colorCode = "ff0000",
            spritePath = ""
        }
    },
    {
        name = "custom_speed",
        data = {
            width = 16,
            height = 16,
            colorCode = "ff0000",
            spritePath = "",
            speedMultiplier = 1.0
        }
    }
}

local options = {
    defaultCogTexture = "objects/zipmover/cog",
    defaultLightsTexture = "objects/zipmover/light01",
    defaultBlockTexture = "objects/zipmover/block",
}

syncedZipMover.sprite = zipMoverHelper.getSpriteFunction(options)
syncedZipMover.selection = zipMoverHelper.getSelectionFunction(options)

return syncedZipMover
