using Chroma.Graphics;
using FirstSampleGame.Abstract;
using System.Numerics;
namespace FirstSampleGame.Entities;

public class HealthMeter : GameEntity
{
	// entity the meter is attached to
	public GameEntity parent = null;

	// meter color variables
	private Color borderColor;
	private Color backgroundColor;
	private Color fillColor;
	private Color defaultFillColor;
	private Color warningFillColor;
	private Color criticalFillColor;

	// meter dimensions variables
	private float meterWidth;
	private float meterHeight;

	public HealthMeter()
	{
		Scale = new(1f);

		// dimensions of the meter
		meterWidth = 35f;
		meterHeight = 6f;

		// meter color definitions
		borderColor = Color.White;
		backgroundColor = Color.Black;
		fillColor = defaultFillColor;
		defaultFillColor = Color.Green;
		warningFillColor = Color.Yellow;
		criticalFillColor = Color.Red;
	}

	public void drawHealthMeter(RenderContext context)
	{

		float healthPct = (float)(parent.HitPoints - parent.Damage) / parent.HitPoints;

		Vector2 meterPos = parent.Position;
		;
		float fillWidth = healthPct * meterWidth;

		meterPos.X -= meterWidth / 2;
		meterPos.Y -= meterHeight / 2;
		meterPos.Y -= (parent.Texture.Height * parent.Scale.X * .5f + meterHeight);

		// set meter color based on healthPct
		fillColor = defaultFillColor;
		if (healthPct < .66f) fillColor = warningFillColor;
		if (healthPct < .33f) fillColor = criticalFillColor;

		// draw meter border, background, fill
		context.Rectangle(
			ShapeMode.Stroke,
			meterPos.X,
			meterPos.Y,
			meterWidth,
			meterHeight,
			borderColor
			);

		context.Rectangle(
			ShapeMode.Fill,
			new Vector2(meterPos.X + 1, meterPos.Y + 1),
			meterWidth - 2,
			meterHeight - 2,
			backgroundColor
		);

		context.Rectangle(
			ShapeMode.Fill,
			new Vector2(meterPos.X + 1, meterPos.Y + 1),
			fillWidth - 2,
			meterHeight - 2,
			fillColor
		);

	}

}