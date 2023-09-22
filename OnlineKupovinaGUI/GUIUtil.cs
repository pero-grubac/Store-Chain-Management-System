namespace OnlineKupovinaGUI
{
    public static class GUIUtil
    {
        // Dozvoljava vracanje unazad
        public static void HideCurrentShowNewForm(Form currentForm, Form newForm)
        {
            currentForm.Hide();
            newForm.Show();
            newForm.Closed += (s, e) => currentForm.Show();
        }

        // Ne dozvoljava vracanje unazad
        public static void ShowNextForm(Form currentForm, Form newForm)
        {
            currentForm.Hide();
            newForm.Show();
        }

    }
}
