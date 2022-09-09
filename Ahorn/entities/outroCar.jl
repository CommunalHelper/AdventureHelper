module AdventureHelperOutroCar

using ..Ahorn, Maple

@mapdef Entity "AdventureHelper/OutroCar" OutroCar(x::Integer, y::Integer, hasRoadAndBarriers::Bool=false)

const placements = Ahorn.PlacementDict(
    "Outro Car (Adventure Helper)" => Ahorn.EntityPlacement(
        OutroCar
    )
)

barrierSprite = "scenery/car/barrier"
bodySprite = "scenery/car/body"
pavementSprite = "scenery/car/pavement"
wheelsSprite = "scenery/car/wheels"

function Ahorn.selection(entity::OutroCar)
    x, y = Ahorn.position(entity)

    hasRoadAndBarriers = get(entity.data, "hasRoadAndBarriers", false)

    rectangles = Ahorn.Rectangle[
        Ahorn.getSpriteRectangle(bodySprite, x, y, jx=0.5, jy=1.0),
        Ahorn.getSpriteRectangle(wheelsSprite, x, y, jx=0.5, jy=1.0),
    ]

    if hasRoadAndBarriers
        push!(rectangles, Ahorn.getSpriteRectangle(barrierSprite, x + 32, y, jx=0.0, jy=1.0))
        push!(rectangles, Ahorn.getSpriteRectangle(barrierSprite, x + 41, y, jx=0.0, jy=1.0))
    end

    return Ahorn.coverRectangles(rectangles)
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::OutroCar, room::Maple.Room)
    x, y = Ahorn.position(entity)

    hasRoadAndBarriers = get(entity.data, "hasRoadAndBarriers", false)
    rng = Ahorn.getSimpleEntityRng(entity)

    pavementWidth = x - 48
    columns = floor(Int, pavementWidth / 8)

    Ahorn.drawSprite(ctx, wheelsSprite, x + 3, y, jx=0.5, jy=1.0, sx=-1)
    Ahorn.drawSprite(ctx, bodySprite, x + 3, y, jx=0.5, jy=1.0, sx=-1)

    if hasRoadAndBarriers
        Ahorn.drawSprite(ctx, barrierSprite, x + 32, y, jx=0.0, jy=1.0)
        Ahorn.drawSprite(ctx, barrierSprite, x + 41, y, jx=0.0, jy=1.0)

        for col in 0:columns - 1
            choice = col >= columns - 2 ? (col != columns - 2 ? 3 : 2) : rand(rng, 0:2)

            Ahorn.drawImage(ctx, pavementSprite, col * 8, y, choice * 8, 0, 8, 4)
        end
    end
end

end