import numpy as np
import matplotlib.pyplot as plt
from scipy.interpolate import BSpline
from mpl_toolkits.mplot3d import Axes3D
import random
import matplotlib.animation as animation

def spherical_to_cartesian(r, theta, phi):
    """Átalakítás gömbi koordinátákból derékszögű koordinátákba"""
    x = r * np.sin(phi) * np.cos(theta)
    y = r * np.sin(phi) * np.sin(theta)
    z = r * np.cos(phi)
    return np.array([x, y, z])

# Felhasználói input kezelése
while True:
    try:
        num_points = int(input("Hány kontrollpont legyen? (min. 8): "))
        if num_points < 8:
            print("Legalább 8 pont szükséges!")
            continue
        break
    except ValueError:
        print("Érvénytelen szám, próbáld újra!")

control_points = []
use_random = input("Szeretnéd véletlenszerűen generálni a pontokat? (igen/nem): ").strip().lower()
if use_random == "igen":
    for _ in range(num_points):
        r = 1  # Sugár fixen 1
        theta = random.uniform(0, 2 * np.pi)
        phi = random.uniform(0, np.pi)
        control_points.append([r, theta, phi])
else:
    print("Add meg a kontrollpontokat (r, theta, phi) formában!")
    for i in range(num_points):
        while True:
            try:
                r, theta, phi = map(float, input(f"Pont {i+1}: ").split())
                control_points.append([r, theta, phi])
                break
            except ValueError:
                print("Hibás formátum! Kérlek, három számot adj meg szóközzel elválasztva!")

# B-spline csomópontok generálása
k = 3  # fok
knots = np.concatenate(([0] * k, np.linspace(0, 1, num_points - k), [1] * k))

# B-spline létrehozása
spl = BSpline(knots, np.array(control_points), k)

# Görbepontok számítása
t_values = np.linspace(0, 1, 100)
curve_points = np.array([spl(t) for t in t_values])

# Átalakítás derékszögű koordinátákra
cartesian_points = np.array([spherical_to_cartesian(r, theta, phi) for r, theta, phi in curve_points])
control_cartesian = np.array([spherical_to_cartesian(*p) for p in control_points])

# 3D megjelenítés
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.plot(cartesian_points[:, 0], cartesian_points[:, 1], cartesian_points[:, 2], label="B-spline görbe")
ax.scatter(control_cartesian[:, 0], control_cartesian[:, 1], control_cartesian[:, 2], color='red', label='Kontrollpontok')

# Érintővektorok számítása
for i in range(len(t_values) - 1):
    p1 = cartesian_points[i]
    p2 = cartesian_points[i + 1]
    tangent = (p2 - p1) * 0.2  # Méretezés
    ax.quiver(p1[0], p1[1], p1[2], tangent[0], tangent[1], tangent[2], color='green', length=0.2)

ax.set_xlabel('X')
ax.set_ylabel('Y')
ax.set_zlabel('Z')
ax.set_title("B-spline görbe gömbi koordinátákban")
ax.legend()

# Animáció létrehozása
def update(num, data, line):
    line.set_data(data[:num, :2].T)
    line.set_3d_properties(data[:num, 2])
    return line,

data = np.array(cartesian_points)
line, = ax.plot([], [], [], 'r', label="B-spline görbe")
ani = animation.FuncAnimation(fig, update, frames=len(data), fargs=(data, line), interval=50, blit=False)

plt.show()