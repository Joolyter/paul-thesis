import distutils.dir_util
import importlib.util
import subprocess
from pathlib import Path


SPHINX_FILES = Path(__file__).parent / 'sphinx_quickstart_files'
SCRIPTS_PATH = Path(__file__).parent.parent / 'joolyterdemo'
DEFAULT_DOCS_DIR_PATH = Path(__file__).parent.parent / 'docs'
THEME = 'sphinx_rtd_theme'
MAKE = 'make.bat'
SHELL = True


def generate_sphinx_docs(docs_path: Path) -> None:
    """ Generates html documentation from the docstrings in the Joolyter package."""
    if importlib.util.find_spec(THEME) is None:
        raise ImportError("\'sphinx_rtd_theme\' package must be installed in order to generate sphinx-documentation.")
    if docs_path.exists():
        _update_sphinx_docs(docs_path)
    else:
        _create_new_sphinx_docs(docs_path)
    # _create_new_sphinx_docs(docs_path)


def _create_new_sphinx_docs(docs_path: Path) -> None:
    docs_path.mkdir(parents=True, exist_ok=True)
    subprocess.run(
        ['sphinx-quickstart', '--ext-autodoc', '-l', 'en', '-p', '', '-a', '', '-r', '', '--no-sep'],
        cwd=docs_path
    )
    distutils.dir_util.copy_tree(str(SPHINX_FILES), str(docs_path))
    subprocess.run(['sphinx-apidoc', '-o', '.', SCRIPTS_PATH], cwd=docs_path)
    subprocess.run([MAKE, 'html'], cwd=docs_path, shell=SHELL)


def _update_sphinx_docs(docs_path: Path) -> None:
    subprocess.run(['sphinx-apidoc', '-f', '-o', '.', SCRIPTS_PATH], cwd=docs_path)
    subprocess.run([MAKE, 'clean'], cwd=docs_path, shell=SHELL)
    subprocess.run([MAKE, 'html'], cwd=docs_path, shell=SHELL)


def main() -> None:
    """ Runs the "generate_sphinx_docs" method to generate the python documentation."""
    generate_sphinx_docs(DEFAULT_DOCS_DIR_PATH)
    path_to_index_html_file = DEFAULT_DOCS_DIR_PATH / '_build' / 'html' / 'index.html'
    print("Generated documentation at:", path_to_index_html_file)


if __name__ == '__main__':
    main()
