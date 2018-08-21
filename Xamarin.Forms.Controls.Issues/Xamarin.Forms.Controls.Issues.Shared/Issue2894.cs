using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2894, "[iOS] Gesture Recognizers added to Span after it's been set to FormattedText don't work and can cause an NRE",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Gestures)]
#endif
	public class Issue2894 : TestContentPage
	{
		Label label = null;

		TapGestureRecognizer CreateRecognizer1() => new TapGestureRecognizer()
		{
			Command = new Command(async () =>
			{
				await DisplayAlert("I fired too", "I fired too", "I fired too");
			})
		};

		TapGestureRecognizer CreateRecognizer2() => new TapGestureRecognizer()
		{
			Command = new Command(async () =>
			{
				await DisplayAlert("Yay clicked", "yay clicked", "yay clicked");
			})
		};

		void AddRemoveSpan()
		{
			if(label.FormattedText != null)
			{
				label.FormattedText = null;
				return;
			}

			FormattedString s = new FormattedString();

			var span = new Span { Text = "I will fire when clicked. ", FontAttributes = FontAttributes.Bold };
			var span2 = new Span { Text = "I should also fire when clicked", FontAttributes = FontAttributes.Bold };

			var recognizer = new TapGestureRecognizer()
			{
				Command = new Command(async () =>
				{
					await DisplayAlert("I fired too", "I fired too", "I fired too");
				})
			};

			span.GestureRecognizers.Add(new TapGestureRecognizer()
			{
				Command = new Command(async () =>
				{
					await DisplayAlert("Yay clicked", "yay clicked", "yay clicked");
				})
			});

			s.Spans.Add(span);
			s.Spans.Add(span2);

			label.FormattedText = s;
			span2.GestureRecognizers.Add(recognizer);
		}

		protected override void Init()
		{
		 	label = new Label();
			AddRemoveSpan();

			Content = new ContentView()
			{
				Content = new StackLayout()
				{
					Children = 
					{ 
						label,
						new Button()
						{
							Text = "Click twice then test again",
							AutomationId = "AddRemoveSpans",
							Command = new Command(() =>
							{
								AddRemoveSpan();
							})
						},
						new Button()
						{
							Text = "Click twice then test again",
							AutomationId = "AddRemoveGestures",
							Command = new Command(async () =>
							{
								if(label.FormattedText == null)
									AddRemoveSpan();

								if(label.FormattedText.Spans[0].GestureRecognizers.Count > 0)
								{
									label.FormattedText.Spans[0].GestureRecognizers.Clear();
									label.FormattedText.Spans[1].GestureRecognizers.Clear();
								}

								await Task.Delay(100);

								label.FormattedText.Spans[0].GestureRecognizers.Add(CreateRecognizer1());
								label.FormattedText.Spans[1].GestureRecognizers.Add(CreateRecognizer2());
							})
						}
					},
					Padding = 40
				}
			};
		}

#if UITEST
		[Test]
		public void GestureWorking()
		{

		}
#endif
	}
}
