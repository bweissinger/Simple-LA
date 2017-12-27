# Simple-LA
A basic C# Linear Algebra library created for the Udacity Linear Algebra Refresher course, as well as a future Android project.

Simple_LA can be used for matrix/vector operations and solving linear systems.

Usage
-----
Matrix operation examples (vector operations follow same principles):
```
//m1 - m2
m3 = m1.Subtract(m2);

//Scalar multiplication
m1 = m1.ScalarMultiplication(2.0);

//Transpose matrix
m3 = m1.Transpose();

//Check if element(i, j) is equal to integer
m1.EqualsInt(1, 0, 0);

//Print Matrix
m1.PrintElements();
```

Linear system examples:
```
//To initialize a new linear system provide an array of LinearEquations
Plane p1 = new Plane(.786,.786,.588,-.714);
			Plane p2 = new Plane(-.131,-.131,.244,.319);
			Plane p3 = new Plane(-2.158,3.01,-1.727,-.831);
			Plane p4 = new Plane(-0.561,-1.056,5.619,5.973);
      
LinearSystem system = new LinearSystem(new Plane[] { p1, p2, p3, p4 });

//To solve for solutions call GaussianElimination() (will print solution if any exist)
system.GaussianElimination();
```
