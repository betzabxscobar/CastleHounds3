public sealed class Challenge04GameBridge : ChallengeGameController
{
    protected override string FallbackChallengeId => "house_challenge_04";

    protected override void OnChallengeStarted()
    {
        Challenge04SceneFlowController.EnterPotionScene(ChallengeId);
    }
}
