local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")
local atlases = require("atlases")

local zipMoverRenderer = {}

local blockNinePatchOptions = {
    mode = "border"
}

local function getTextureOrDefault(customPath, fallback)
    local spriteMeta = atlases.getResource(customPath, "Gameplay")
    if spriteMeta then
        return customPath
    end

    spriteMeta = atlases.getResource(fallback, "Gameplay")
    if spriteMeta then
        return fallback
    end
end

local function getTextureCache(directory, options)
    options.cache = options.cache or {}

    local cached = options.cache[directory]
    if cached then
        return cached
    end

    local entry = {
        cog = getTextureOrDefault(directory .. "/cog", options.defaultCogTexture),
        lights = getTextureOrDefault(directory .. "/light01", options.defaultLightsTexture),
        block = getTextureOrDefault(directory .. "/block", options.defaultBlockTexture),
    }

    local cogMeta = atlases.getResource(entry.cog, "Gameplay")
    entry.cogWidth = cogMeta.width
    entry.cogHeight = cogMeta.height

    options.cache[directory] = entry

    return entry
end

local function addNodeSprites(sprites, entity, cogTexture, centerX, centerY, centerNodeX, centerNodeY, options)
    local nodeCogSprite = drawableSprite.fromTexture(cogTexture, entity)

    nodeCogSprite:setPosition(centerNodeX, centerNodeY)
    nodeCogSprite:setJustification(0.5, 0.5)

    local ropeColor = entity.colorCode or options.defaultRopeColor or "ff0000"

    local points = {centerX, centerY, centerNodeX, centerNodeY}
    local leftLine = drawableLine.fromPoints(points, ropeColor, 1)
    local rightLine = drawableLine.fromPoints(points, ropeColor, 1)

    leftLine:setOffset(0, 4.5)
    rightLine:setOffset(0, -4.5)

    leftLine.depth = 5000
    rightLine.depth = 5000

    for _, sprite in ipairs(leftLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    for _, sprite in ipairs(rightLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    table.insert(sprites, nodeCogSprite)
end

local function addBlockSprites(sprites, entity, blockTexture, lightsTexture, x, y, width, height)
    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, {0.0, 0.0, 0.0})
    local frameNinePatch = drawableNinePatch.fromTexture(blockTexture, blockNinePatchOptions, x, y, width, height)
    local lightsSprite = drawableSprite.fromTexture(lightsTexture, entity)

    lightsSprite:addPosition(math.floor(width / 2), 0)
    lightsSprite:setJustification(0.5, 0.0)

    table.insert(sprites, rectangle)
    table.insert(sprites, frameNinePatch)
    table.insert(sprites, lightsSprite)
end

function zipMoverRenderer.getSpriteFunction(options)
    return function (room, entity)
        local sprites = {}

        local x, y = entity.x or 0, entity.y or 0
        local width, height = entity.width or 16, entity.height or 16
        local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)

        local nodes = entity.nodes or {{x = 0, y = 0}}
        local nodeX, nodeY = nodes[1].x, nodes[1].y

        local centerX, centerY = x + halfWidth, y + halfHeight
        local centerNodeX, centerNodeY = nodeX + halfWidth, nodeY + halfHeight

        local spritePath = entity.spritePath or ""

        local textureCache = getTextureCache(spritePath, options)
        local blockTexture, lightTexture, cogTexture = textureCache.block, textureCache.lights, textureCache.cog

        addNodeSprites(sprites, entity, cogTexture, centerX, centerY, centerNodeX, centerNodeY, options)
        addBlockSprites(sprites, entity, blockTexture, lightTexture, x, y, width, height)

        return sprites
    end
end

function zipMoverRenderer.getSelectionFunction(options)
    return function(room, entity)
        local x, y = entity.x or 0, entity.y or 0
        local width, height = entity.width or 8, entity.height or 8
        local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)

        local node = entity.nodes and entity.nodes[1] or {x = 0, y = 0}
        local centerNodeX, centerNodeY = node.x + halfWidth, node.y + halfHeight

        local spritePath = entity.spritePath or ""
        local textureCache = getTextureCache(spritePath, options)

        local cogWidth, cogHeight = textureCache.cogWidth, textureCache.cogHeight

        local mainRectangle = utils.rectangle(x, y, width, height)
        local nodeRectangle = utils.rectangle(centerNodeX - math.floor(cogWidth / 2), centerNodeY - math.floor(cogHeight / 2), cogWidth, cogHeight)

        return mainRectangle, {nodeRectangle}
    end
end

return zipMoverRenderer
