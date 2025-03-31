import numpy as np
import matplotlib.pyplot as plt
from scipy.interpolate import BSpline

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

# 3D megjelenítés
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.plot(cartesian_points[:, 0], cartesian_points[:, 1], cartesian_points[:, 2], label="B-spline görbe")
ax.scatter(*zip(*[spherical_to_cartesian(*p) for p in control_points]), color='red', label='Kontrollpontok')

ax.set_xlabel('X')
ax.set_ylabel('Y')
ax.set_zlabel('Z')
ax.set_title("B-spline görbe gömbi koordinátákban")
ax.legend()

plt.show()
