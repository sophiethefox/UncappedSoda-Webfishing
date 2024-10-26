using GDWeave;
using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace UncappedSoda;

/**
 * Code partially from https://github.com/NotNite/WebfishingPlus/blob/main/WebfishingPlus/Mods/MenuTweaks.cs
 */
public class PlayerPatch : IScriptMod
{
    
    public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

    public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
    {
        //  "growth": player_scale =
        var growthMatch = new MultiTokenWaiter([
            t => t is ConstantToken { Value: StringVariant { Value: "growth" } },
            t => t.Type is TokenType.Colon,
            t => t is IdentifierToken { Name: "player_scale" },
            t => t.Type is TokenType.OpAssign
        ]);
        //  "shrink": player_scale =
        var shrinkMatch = new MultiTokenWaiter([
            t => t is ConstantToken { Value: StringVariant { Value: "shrink" } },
            t => t.Type is TokenType.Colon,
            t => t is IdentifierToken { Name: "player_scale" },
            t => t.Type is TokenType.OpAssign
        ]);
        var newlineConsumer = new TokenConsumer(t => t.Type is TokenType.Newline);

        foreach (var token in tokens)
        {
            if (newlineConsumer.Check(token))
            {
                continue;
            }

            if (newlineConsumer.Ready)
            {
                yield return token;
                newlineConsumer.Reset();
            }

            if (growthMatch.Check(token))
            {
                yield return token;
                yield return new IdentifierToken("player_scale");
                yield return new Token(TokenType.OpAdd);
                yield return new ConstantToken(new RealVariant(0.5));

                // Reset since this matches multiple times
                growthMatch.Reset();
                // Consume the "disabled or refreshing" and wait until newline
                newlineConsumer.SetReady();
            } else if (shrinkMatch.Check(token))
            {
                yield return token;
                yield return new IdentifierToken("player_scale");
                yield return new Token(TokenType.OpSub);
                yield return new ConstantToken(new RealVariant(0.1));

                // Reset since this matches multiple times
                growthMatch.Reset();
                // Consume the "disabled or refreshing" and wait until newline
                newlineConsumer.SetReady();
            }
            else
            {
                yield return token;
            }
        }
    }
}

