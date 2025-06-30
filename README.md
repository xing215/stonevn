# Ô Ăn Quan (Stone Eating Game) - AI Implementation

A Unity-based implementation of the traditional Vietnamese board game "Ô Ăn Quan" featuring an intelligent AI opponent powered by the Minimax algorithm with Alpha-Beta pruning.

##   Project Information

- **University**: Vietnam National University Ho Chi Minh City - University of Science (VNU-HCMUS)  
- **Semester**: 5th Semester  
- **Course**: Introduction to Artificial Intelligence  
- **Project Type**: Team Project  
- **Language**: C#  
- **Game Engine**: Unity 6000.1.1f1  
- **Target Platform**: WebGL  

## 👥 Team Members
- @xing215
- @nickhuy1809
- @z3nz3nn
- @Millicos

## 📖 About the Project

This project is developed as part of the **Introduction to Artificial Intelligence** course at Vietnam National University Ho Chi Minh City - University of Science (VNU-HCMUS). The game demonstrates practical applications of AI algorithms in game development, specifically implementing adversarial search techniques.

### Game Overview

**Ô Ăn Quan** (also known as "Stone Eating Game" or "Mandarin's Box") is a traditional Vietnamese strategy board game similar to Mancala. The game involves strategic thinking and planning as players move stones around the board to capture their opponent's pieces.

## 🎮 Game Features

### Core Gameplay
- **Traditional Rules**: Authentic implementation of Ô Ăn Quan gameplay
- **Interactive UI**: User-friendly Unity-based interface
- **Real-time Animation**: Smooth stone movement animations
- **Score Tracking**: Live score updates for both player and AI

### AI Features
- **Minimax Algorithm**: Intelligent decision-making using game tree search
- **Alpha-Beta Pruning**: Optimized search for better performance
- **Difficulty Scaling**: Adjustable AI depth for different challenge levels
- **Strategic Evaluation**: Heuristic board evaluation function

### Special Game Mechanics
- **Card System**: Special action cards that modify gameplay
  - **Reverse Turn**: Changes the direction of play
  - **Skip Turn**: Skips opponent's turn
  - **Extra Turn**: Grants an additional turn
- **Dynamic Scoring**: Real-time score calculation and updates
- **End Game Detection**: Automatic game termination and final scoring

## 🛠️ Technical Implementation

### Technologies Used
- **Unity 6000.1.1f1**: Game engine and development platform
- **C# (.NET Standard 2.1)**: Primary programming language
- **WebGL**: Target platform for web deployment
- **Unity Input System**: Modern input handling

### AI Algorithm Details

#### Minimax with Alpha-Beta Pruning
```csharp
private int Minimax(SimState state, int depth, bool isMaximizingPlayer, int alpha, int beta)
```

The AI uses a minimax algorithm with alpha-beta pruning to evaluate possible moves and select the optimal strategy. Key features:

- **Search Depth**: Configurable depth (default: 3 levels)
- **Evaluation Function**: Considers stone count, positioning, and strategic advantages
- **Pruning Optimization**: Reduces search space by up to 50% using alpha-beta cuts
- **State Simulation**: Efficient game state simulation for move evaluation

#### Board Evaluation Heuristics
- Stone count differential between AI and player
- Positional advantages on the board
- Potential capture opportunities
- End-game scenarios evaluation

## 🚀 Getting Started

### Prerequisites
- Unity 6000.1.1f1 or later
- Modern web browser (for WebGL build)
- Basic understanding of Ô Ăn Quan rules

### Installation & Running

1. **Clone the Repository**
   ```bash
   git clone [repository-url]
   cd stonevn
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Open the project folder `src/`
   - Wait for Unity to import assets

3. **Build and Run**
   - Open the scene `Assets/Scenes/PlayView.unity`
   - Build for WebGL or play directly in Unity Editor
   - For web deployment, use the pre-built files in `build/`

### Web Deployment
The project includes a pre-built WebGL version in the `build/` directory. Simply serve the files using any web server:

```bash
# Using Python (example)
cd build
python -m http.server 8000
```

Then navigate to `http://localhost:8000` in your browser.

## 🎯 Game Rules

### Basic Gameplay
1. **Board Setup**: 12 cells arranged in a rectangle, with "quan" (mandarin) pieces at positions 0 and 6
2. **Starting Stones**: Each regular cell starts with 5 stones, quan cells with 10 stones
3. **Turn System**: Players alternate turns, with humans controlling cells 1-5 and AI controlling cells 7-11

### Winning Conditions
- Game ends when one side has no valid moves or all quan pieces are captured
- Player with the highest score (captured stones) wins
- Strategic stone placement and timing are crucial for victory

### Special Cards
Players can earn and use special cards to gain tactical advantages:
- Use cards strategically to disrupt opponent's plans
- Cards are earned through successful captures

## 🏗️ Project Structure

```
stonevn/
├── build/                  # WebGL build output
│   ├── index.html         # Main web page
│   ├── Build/             # Unity WebGL files
│   └── TemplateData/      # Web assets
├── src/                   # Unity project source
│   ├── Assets/
│   │   ├── Scripts/       # C# game logic
│   │   │   ├── BoardManager.cs    # Core game logic & AI
│   │   │   ├── Cell.cs           # Board cell management
│   │   │   ├── GUIManager.cs     # UI management
│   │   │   └── ...
│   │   ├── Scenes/        # Unity scenes
│   │   ├── Prefabs/       # Game object prefabs
│   │   └── UI/            # User interface assets
│   └── ProjectSettings/   # Unity project configuration
└── README.md
```

## 🧠 AI Implementation Details

### Algorithm Choice: Minimax with Alpha-Beta Pruning

The AI implementation uses the Minimax algorithm enhanced with Alpha-Beta pruning for the following reasons:

1. **Perfect Information**: Ô Ăn Quan is a perfect information game, making Minimax ideal
2. **Zero-Sum Nature**: One player's gain is exactly the other's loss
3. **Performance**: Alpha-Beta pruning significantly reduces computation time
4. **Deterministic**: Provides consistent and predictable AI behavior

### Performance Optimizations
- **State Simulation**: Efficient board state copying for move evaluation
- **Move Ordering**: Prioritizes promising moves to improve pruning effectiveness
- **Depth Limiting**: Configurable search depth balances strength vs. response time
- **Iterative Deepening**: Could be implemented for time-constrained scenarios

## 🎓 Learning Objectives

This project demonstrates understanding of:

### AI Concepts
- **Adversarial Search**: Minimax algorithm implementation
- **Game Tree Search**: Systematic exploration of game states
- **Heuristic Evaluation**: Designing effective board evaluation functions
- **Optimization Techniques**: Alpha-Beta pruning for performance

### Software Engineering
- **Object-Oriented Design**: Clean code architecture in C#
- **Unity Development**: Game engine utilization and optimization
- **State Management**: Complex game state handling
- **User Interface Design**: Intuitive game interface creation

## 📚 References & Resources

### Academic Sources
- "Artificial Intelligence: A Modern Approach" by Russell & Norvig
- Game AI programming techniques and best practices
- Traditional Vietnamese board games documentation

### Technical Documentation
- Unity Game Engine Documentation
- C# Programming Language Specification
- WebGL Deployment Guidelines

## 🏆 Future Enhancements

### Potential Improvements
- **Machine Learning**: Neural network-based AI evaluation
- **Multiplayer Support**: Online multiplayer capabilities
- **Mobile Optimization**: Touch-friendly interface for mobile devices
- **Tutorial System**: Interactive learning mode for new players
- **Statistics Tracking**: Detailed game analytics and player progression

### Advanced AI Features
- **Opening Book**: Pre-computed optimal opening moves
- **Endgame Database**: Perfect play in endgame scenarios
- **Adaptive Difficulty**: AI that adjusts to player skill level
- **Multiple AI Personalities**: Different playing styles and strategies

---

**This project was developed as part of the 5th semester coursework at VNU-HCMUS.**