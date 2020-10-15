﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace JiME
{
	public class ThreatInteraction : INotifyPropertyChanged, ICommonData, IInteraction
	{
		//common
		string _dataName, _triggerName, _triggerAfterName, _triggerDefeatedName;
		bool _isTokenInteraction;
		bool[] _includedEnemies;
		int _loreReward, _basePoolPoints;
		TokenType _tokenType;
		PersonType _personType;
		DifficultyBias _difficultyBias;

		public string dataName
		{
			get { return _dataName; }
			set
			{
				if ( _dataName != value )
				{
					_dataName = value;
					NotifyPropertyChanged( "dataName" );
				}
			}
		}
		public Guid GUID { get; set; }
		public bool isEmpty { get; set; }
		public string triggerName
		{
			get => _triggerName;
			set
			{
				_triggerName = value;
				NotifyPropertyChanged( "triggerName" );
			}
		}
		public string triggerAfterName
		{
			get => _triggerAfterName;
			set
			{
				_triggerAfterName = value;
				NotifyPropertyChanged( "triggerAfterName" );
			}
		}
		public bool isTokenInteraction
		{
			get => _isTokenInteraction;
			set
			{
				_isTokenInteraction = value;
				NotifyPropertyChanged( "isTokenInteraction" );
			}
		}
		public TokenType tokenType
		{
			get => _tokenType;
			set
			{
				_tokenType = value;
				NotifyPropertyChanged( "tokenType" );
			}
		}
		public PersonType personType
		{
			get => _personType;
			set { _personType = value; NotifyPropertyChanged( "personType" ); }
		}
		public TextBookData textBookData { get; set; }
		public TextBookData eventBookData { get; set; }
		public int loreReward
		{
			get => _loreReward;
			set
			{
				_loreReward = value;
				NotifyPropertyChanged( "loreReward" );
			}
		}
		public string triggerDefeatedName
		{
			get { return _triggerDefeatedName; }
			set
			{
				_triggerDefeatedName = value;
				NotifyPropertyChanged( "triggerDefeatedName" );
			}
		}
		public int basePoolPoints
		{
			get => _basePoolPoints;
			set
			{
				_basePoolPoints = value;
				NotifyPropertyChanged( "basePoolPoints" );
			}
		}
		public bool[] includedEnemies
		{
			get => _includedEnemies;
			set
			{
				_includedEnemies = value;
				NotifyPropertyChanged( "includedEnemies" );
			}
		}
		public DifficultyBias difficultyBias
		{
			get => _difficultyBias;
			set
			{
				_difficultyBias = value;
				NotifyPropertyChanged( "difficultyBias" );
			}
		}


		//IInteraction properties
		public InteractionType interactionType { get; set; }

		public ObservableCollection<Monster> monsterCollection { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public ThreatInteraction( string name, bool random )
		{
			interactionType = InteractionType.Threat;
			dataName = name;
			GUID = Guid.NewGuid();
			isEmpty = false;
			triggerName = "None";
			triggerAfterName = "None";
			triggerDefeatedName = "None";
			isTokenInteraction = false;
			tokenType = TokenType.None;
			personType = PersonType.Human;
			textBookData = new TextBookData();
			textBookData.pages.Add( "Default Flavor Text\n\nUse this text to describe the Event situation and present choices, depending on the type of Event this is." );
			eventBookData = new TextBookData();
			eventBookData.pages.Add( "Default Event Text.\n\nThis text is shown after the Event is triggered. Use it to tell about the actual event that has been triggered Example: Describe an Enemy Threat, present a Test, describe a Decision, etc." );
			loreReward = 0;
			basePoolPoints = 10;
			includedEnemies = new bool[7].Fill( true );
			difficultyBias = DifficultyBias.Medium;

			monsterCollection = new ObservableCollection<Monster>();
		}

		public void NotifyPropertyChanged( string propName )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propName ) );
		}

		public void RenameTrigger( string oldName, string newName )
		{
			if ( triggerName == oldName )
				triggerName = newName;

			if ( triggerAfterName == oldName )
				triggerAfterName = newName;

			if ( triggerDefeatedName == oldName )
				triggerDefeatedName = newName;
		}

		public void AddMonster( Monster m )
		{
			monsterCollection.Add( m );
		}
	}
}
