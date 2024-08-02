# Prey and Predator Machine Learning Simulation 

## Description
This project is a machine learning simulation that explores the dynamics between prey and predator agents within a controlled environment. Using Unity's ML-Agents toolkit, two AI agents—a deer (prey) and a tiger (predator)—are trained with reinforcement learning algorithms to survive and achieve their goals. The simulation provides insights into how AI agents can learn complex behaviors through interaction with their environment and each other.

## Key Features
- **Reinforcement Learning:** Both the prey and predator are trained using reinforcement learning techniques, learning to maximize their rewards through trial and error.
- **Agent Behaviors:** The prey learns to collect pellets and evade the predator, while the predator learns to track and catch the prey. The behavior of both agents evolves over time as they learn from their environment and each other.
- **Dynamic Environment:** The environment is randomly generated at the start of each episode, including shrubs, walls, and pellets that agents interact with, impacting their decision-making and strategies.
- **Hunger System:** Agents have a hunger timer that affects their behavior, requiring them to collect food (prey) or catch the prey (predator) to survive.
- **Camera Perspectives:** The simulation includes multiple camera angles to view the environment from different perspectives, including overhead, side views, and the agent's point of view (POV).

## About the Project
I built this project from scratch using the ML-Agents package in Unity, driven by my passion for machine learning and artificial intelligence. Everything you see here is the result of self-learning, building on the skills I've developed in game development. This project is not a game but a simulation. I chose C#/Unity because it offers a visual interface.

## Learning Process
The agents in this simulation learn through reinforcement learning, a process where they interact with their environment, make decisions, and receive rewards or penalties based on their actions. Over time, these interactions help the agents improve their behavior as their neural networks are updated with each experience.

## Reinforcement Learning Parameters
Each agent is equipped with basic movement and vision capabilities using ray sensors. 
(Ray sensors are like virtual eyes for the agents—they cast invisible rays in different directions to detect obstacles, walls, and other objects in the environment.)

### **Prey (Deer)**
- **Traits:** 
  - The prey is slightly faster than the predator.
  - The prey has a wider vision, allowing it to see more of its surroundings.
- **Objective:** The prey's goal is to collect as many blue pellets as possible while avoiding obstacles and evading the predator.

**Rewards:**
- Rewarded for collecting a pellet.
- Rewarded for collecting all pellets.

**Penalties:**
- Penalized for running into a wall.
- Penalized for being hunted by the predator.
- Penalized for starving.

### **Predator (Tiger)**
- **Traits:** 
  - The predator can attack the prey.
  - The predator has a longer vision range, allowing it to see farther.
- **Objective:** The tiger's goal is to hunt the prey while surviving as long as possible.

**Rewards:**
- Rewarded for successfully hunting the prey.

**Penalties:**
- Penalized for hitting a wall.
- Penalized for starving.

## Discoveries During Simulation
I ran a 1,000,000-step training session and found some pretty interesting things along the way. Check out the clips from each step [here](https://drive.google.com/file/d/1tRmuYEN2a70zLzCFZwxRI4D_R0hqJYjp/view?usp=drive_link).

### Step 250,000
**Prey (Deer)**
- The deer has hesitantly learned to seek out the blue pellets.
- Once the deer spots a pellet, it reliably goes for it.

**Predator (Tiger)**
- The tiger is still uncertain about its goal and lacks clear direction.

### Step 500,000
**Prey (Deer)**
- The deer has started to develop some bad habits, such as moving backward frequently.
- Its decision-making has worsened due to the fear of being hunted by the tiger.

**Predator (Tiger)**
- The tiger has become more decisive in its actions.

### Step 750,000
**Prey (Deer)**
- The deer has leaned into its bad habits, realizing that it can move backward at the same speed while keeping an eye on the tiger.
- It learned to effectively run from the tiger by constantly looking at it and moving backward. However, this hurt its overall reward because it was more focused on avoiding the tiger than collecting pellets.

**Predator (Tiger)**
- The tiger developed a strategy to hunt the deer more effectively by waiting near the blue pellets. It learned that the deer would eventually come to the pellets, so it would wait by them.

### Step 1,000,000
**Prey (Deer)**
- By this step, the deer realized that its backward running strategy was not efficient and returned to focusing on gathering pellets.

**Predator (Tiger)**
- The tiger continued with its strategy of hanging around the blue pellets, but it refined its approach by circling the pellets to locate the deer more easily.

By step 1,000,000, both agents had found the most efficient way to exist in the environment they were placed in.

## How to Run
You can download a zip of the built project from this [link](https://drive.google.com/file/d/1AWtH4ZbsOXxY0pNy-b7MkI9A6Ywi9g-Z/view?usp=drive_link). The agents' models included in the build are the highest-performing versions achieved through training.

## Preview
- **Main Menu:** [Preview the main menu here](https://drive.google.com/file/d/12yy0RJUTBPrJ8OTTE6zPkudSpFd4AFgA/view?usp=drive_link)
- **About Page:** [Preview the about page here](https://drive.google.com/file/d/1_JXXQx3bYDSpO4pDlHipU8RrZVquqkWV/view?usp=drive_link)

## Contact
If you have any questions, suggestions, or just want to connect, feel free to reach out to me:

- Email: areyanr@hotmail.com
