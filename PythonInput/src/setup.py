from setuptools import find_packages, setup


setup(
    name="joolyterdemo",
    packages=find_packages(include=["joolyterdemo"]),
    version="0.3.4",
    description="Python package that goes with Joolyter gamification approach.",
    long_description="Joolyter was developed in the course of Bachelor thesis "
                "'A Gamification Approach to the Teaching of Space Flight Mechanics'. "
                "It represents the core application, interpreting solution scripts to use kRPC "
                "to communicate with KSP. \n\n"
                "Source code: https://github.com/Joolyter/paul-thesis/tree/main/PythonInput/src \n"
                "Documentation: https://joolyter.github.io/paul-thesis/PythonInput/src/docs/_build/html/index.html",
    author="Paul J. Schmiedtke",
    license="None",
    install_requires=["protobuf==3.19.4", "setuptools==57.5.0", "krpc", "p2j", "jupyterlab"]
)
