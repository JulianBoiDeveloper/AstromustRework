using UnityEngine;
using OpenAI;

public class WorldInfo : MonoBehaviour
{
    private OpenAIApi openai = new OpenAIApi();
    [SerializeField, TextArea] private string gameStory;
    [SerializeField, TextArea] private string gameWold;
    
    public string GetPrompt()
    {
        return $"Game Story: {gameStory}\n" +
               $"Game World: {gameWold}\n";
    }
}
