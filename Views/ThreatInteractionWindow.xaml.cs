﻿using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace JiME.Views
{
	/// <summary>
	/// Interaction logic for ThreatInteractionWindow.xaml
	/// </summary>
	public partial class ThreatInteractionWindow : Window, INotifyPropertyChanged
	{
		string oldName;

		public Scenario scenario { get; set; }
		public ThreatInteraction interaction { get; set; }
		bool closing = false;

		public event PropertyChangedEventHandler PropertyChanged;
		bool _isThreatTriggered;
		public bool isThreatTriggered
		{
			get => _isThreatTriggered;
			set
			{
				_isThreatTriggered = value;
				PropChanged( "isThreatTriggered" );
			}
		}

		public ThreatInteractionWindow( Scenario s, ThreatInteraction inter = null )
		{
			InitializeComponent();
			DataContext = this;

			scenario = s;
			cancelButton.Visibility = inter == null ? Visibility.Visible : Visibility.Collapsed;
			interaction = inter ?? new ThreatInteraction( "New Threat Event" );

			isThreatTriggered = scenario.threatObserver.Any( x => x.triggerName == interaction.dataName );
			if ( isThreatTriggered )
			{
				addMainTriggerButton.IsEnabled = false;
				triggeredByCB.IsEnabled = false;
				isTokenCB.IsEnabled = false;
				interaction.isTokenInteraction = false;
			}

			if ( interaction.isTokenInteraction && interaction.tokenType == TokenType.Person )
				personType.Visibility = Visibility.Visible;
			humanRadio.IsChecked = interaction.personType == PersonType.Human;
			elfRadio.IsChecked = interaction.personType == PersonType.Elf;
			hobbitRadio.IsChecked = interaction.personType == PersonType.Hobbit;
			dwarfRadio.IsChecked = interaction.personType == PersonType.Dwarf;

			personRadio.IsChecked = interaction.tokenType == TokenType.Person;
			searchRadio.IsChecked = interaction.tokenType == TokenType.Search;
			darkRadio.IsChecked = interaction.tokenType == TokenType.Darkness;
			threatRadio.IsChecked = interaction.tokenType == TokenType.Threat;

			oldName = interaction.dataName;

			rufCB.IsChecked = interaction.includedEnemies[0];
			gobCB.IsChecked = interaction.includedEnemies[1];
			huntCB.IsChecked = interaction.includedEnemies[2];
			marCB.IsChecked = interaction.includedEnemies[3];
			wargCB.IsChecked = interaction.includedEnemies[4];
			hTrollCB.IsChecked = interaction.includedEnemies[5];
			wightCB.IsChecked = interaction.includedEnemies[6];

			biasLight.IsChecked = interaction.difficultyBias == DifficultyBias.Light;
			biasMedium.IsChecked = interaction.difficultyBias == DifficultyBias.Medium;
			biasHeavy.IsChecked = interaction.difficultyBias == DifficultyBias.Heavy;
		}

		private void isTokenCB_Click( object sender, RoutedEventArgs e )
		{
			if ( isTokenCB.IsChecked == true )
			{
				interaction.triggerName = "None";
				personType.Visibility = personRadio.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
			}
			else
				personType.Visibility = Visibility.Collapsed;
		}

		private void EditFlavorButton_Click( object sender, RoutedEventArgs e )
		{
			TextEditorWindow tw = new TextEditorWindow( scenario, EditMode.Flavor, interaction.textBookData );
			if ( tw.ShowDialog() == true )
			{
				interaction.textBookData.pages = tw.textBookController.pages;
				flavorTB.Text = tw.textBookController.pages[0];
			}
		}

		private void EditEventButton_Click( object sender, RoutedEventArgs e )
		{
			TextEditorWindow tw = new TextEditorWindow( scenario, EditMode.Progress, interaction.eventBookData );
			if ( tw.ShowDialog() == true )
			{
				interaction.eventBookData.pages = tw.textBookController.pages;
				eventTB.Text = tw.textBookController.pages[0];
			}
		}

		bool TryClosing()
		{
			//check for dupe name
			if ( interaction.dataName == "New Threat Event" || scenario.interactionObserver.Count( x => x.dataName == interaction.dataName ) > 1 )
			{
				MessageBox.Show( "Give this Event a unique name.", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error );
				return false;
			}

			return true;
		}

		private void Window_Closing( object sender, CancelEventArgs e )
		{
			if ( !closing )
				e.Cancel = true;
		}

		private void addMainTriggerAfterButton_Click( object sender, RoutedEventArgs e )
		{
			TriggerEditorWindow tw = new TriggerEditorWindow( scenario );
			if ( tw.ShowDialog() == true )
			{
				interaction.triggerAfterName = tw.triggerName;
			}
		}

		private void addMainTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			TriggerEditorWindow tw = new TriggerEditorWindow( scenario );
			if ( tw.ShowDialog() == true )
			{
				interaction.triggerName = tw.triggerName;
			}
		}

		private void OkButton_Click( object sender, RoutedEventArgs e )
		{
			if ( !TryClosing() )
				return;

			if ( searchRadio.IsChecked.HasValue && searchRadio.IsChecked.Value )
				interaction.tokenType = TokenType.Search;
			if ( personRadio.IsChecked.HasValue && personRadio.IsChecked.Value )
				interaction.tokenType = TokenType.Person;
			if ( darkRadio.IsChecked.HasValue && darkRadio.IsChecked.Value )
				interaction.tokenType = TokenType.Darkness;
			if ( threatRadio.IsChecked.HasValue && threatRadio.IsChecked.Value )
				interaction.tokenType = TokenType.Threat;

			if ( humanRadio.IsChecked == true )
				interaction.personType = PersonType.Human;
			if ( elfRadio.IsChecked == true )
				interaction.personType = PersonType.Elf;
			if ( hobbitRadio.IsChecked == true )
				interaction.personType = PersonType.Hobbit;
			if ( dwarfRadio.IsChecked == true )
				interaction.personType = PersonType.Dwarf;

			scenario.UpdateEventReferences( oldName, interaction );

			interaction.includedEnemies[0] = rufCB.IsChecked.Value;
			interaction.includedEnemies[1] = gobCB.IsChecked.Value;
			interaction.includedEnemies[2] = huntCB.IsChecked.Value;
			interaction.includedEnemies[3] = marCB.IsChecked.Value;
			interaction.includedEnemies[4] = wargCB.IsChecked.Value;
			interaction.includedEnemies[5] = hTrollCB.IsChecked.Value;
			interaction.includedEnemies[6] = wightCB.IsChecked.Value;

			if ( biasLight.IsChecked == true )
				interaction.difficultyBias = DifficultyBias.Light;
			if ( biasMedium.IsChecked == true )
				interaction.difficultyBias = DifficultyBias.Medium;
			if ( biasHeavy.IsChecked == true )
				interaction.difficultyBias = DifficultyBias.Heavy;

			closing = true;
			DialogResult = true;
		}

		private void CancelButton_Click( object sender, RoutedEventArgs e )
		{
			closing = true;
			DialogResult = false;
		}

		private void Window_ContentRendered( object sender, System.EventArgs e )
		{
			nameTB.Focus();
			nameTB.SelectAll();
		}

		private void AddMonsterButton_Click( object sender, RoutedEventArgs e )
		{
			MonsterEditorWindow me = new MonsterEditorWindow();
			if ( me.ShowDialog() == true )
			{
				interaction.AddMonster( me.monster );
			}
		}

		private void EditButton_Click( object sender, RoutedEventArgs e )
		{
			Monster m = ( (Button)sender ).DataContext as Monster;
			MonsterEditorWindow me = new MonsterEditorWindow( m );
			me.ShowDialog();
		}

		private void DeleteButton_Click( object sender, RoutedEventArgs e )
		{
			Monster m = ( (Button)sender ).DataContext as Monster;
			interaction.monsterCollection.Remove( m );
		}

		void PropChanged( string name )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( name ) );
		}

		private void tokenHelp_Click( object sender, RoutedEventArgs e )
		{
			HelpWindow hw = new HelpWindow( HelpType.Token, 1 );
			hw.ShowDialog();
		}

		private void groupHelp_Click( object sender, RoutedEventArgs e )
		{
			HelpWindow hw = new HelpWindow( HelpType.Grouping );
			hw.ShowDialog();
		}

		private void nameTB_TextChanged( object sender, TextChangedEventArgs e )
		{
			interaction.dataName = ( (TextBox)sender ).Text;
			Regex rx = new Regex( @"\sGRP\d+$" );
			MatchCollection matches = rx.Matches( interaction.dataName );
			if ( matches.Count > 0 )
				groupInfo.Text = "This Event is in the following group: " + matches[0].Value.Trim();
			else
				groupInfo.Text = "This Event is in the following group: None";
		}

		private void addDefeatedTriggerButton_Click( object sender, RoutedEventArgs e )
		{
			TriggerEditorWindow tw = new TriggerEditorWindow( scenario );
			if ( tw.ShowDialog() == true )
			{
				interaction.triggerDefeatedName = tw.triggerName;
			}
		}

		private void help_Click( object sender, RoutedEventArgs e )
		{
			HelpWindow hw = new HelpWindow( HelpType.Enemies, 0 );
			hw.ShowDialog();
		}

		private void tokenTypeClick( object sender, RoutedEventArgs e )
		{
			RadioButton rb = e.Source as RadioButton;
			if ( ( (string)rb.Content ) == "Person" )
				personType.Visibility = Visibility.Visible;
			else
				personType.Visibility = Visibility.Collapsed;
		}

		private void simulateBtn_Click( object sender, RoutedEventArgs e )
		{
			var sd = new SimulatorData()
			{
				poolPoints = interaction.basePoolPoints,
				difficultyBias = biasLight.IsChecked == true ? DifficultyBias.Light : ( biasMedium.IsChecked == true ? DifficultyBias.Medium : DifficultyBias.Heavy )
			};

			sd.includedEnemies[0] = rufCB.IsChecked.Value;
			sd.includedEnemies[1] = gobCB.IsChecked.Value;
			sd.includedEnemies[2] = huntCB.IsChecked.Value;
			sd.includedEnemies[3] = marCB.IsChecked.Value;
			sd.includedEnemies[4] = wargCB.IsChecked.Value;
			sd.includedEnemies[5] = hTrollCB.IsChecked.Value;
			sd.includedEnemies[6] = wightCB.IsChecked.Value;

			var sim = new EnemyCalculator( sd );
			sim.ShowDialog();
		}
	}
}
