import { Dispatch, SetStateAction } from "react";

interface PagePickerProps {
  page: number;
  pages: number;
  setPage: Dispatch<SetStateAction<number>>;
}

function onlyUnique(value: number, index: number, array: number[]) {
  return array.indexOf(value) === index;
}

const PagePicker = ({ page, pages, setPage }: PagePickerProps) => {
  let pagesList: number[] = [page];

  if (page > 1) {
    pagesList = [page - 1, ...pagesList];
  }

  if (page < pages) {
    pagesList = [...pagesList, page + 1];
  }

  if (pagesList.length < 3) {
    if (page - 2 > 0) {
      pagesList = [page - 2, ...pagesList];
    } else if (page + 2 <= pages) {
      pagesList = [...pagesList, page + 2];
    }
  }

  const pagesToDisplay = pagesList.filter(onlyUnique).sort((a, b) => a - b);

  return (
    <nav className="pagination" role="navigation" aria-label="pagination">
      <button
        className="pagination-previous"
        onClick={() => setPage((p) => p - 1)}
        disabled={page === 0}
      >
        Previous
      </button>
      <button
        className="pagination-next"
        onClick={() => setPage((p) => p + 1)}
        disabled={page >= pages - 1}
      >
        Next
      </button>
      <ul className="pagination-list">
        {pagesToDisplay.length > 0 && (
          <li>
            <button
              onClick={() => setPage(0)}
              className={
                page == 0 ? "pagination-link is-current" : "pagination-link"
              }
            >
              1
            </button>
          </li>
        )}

        {pagesToDisplay[0] > 1 && (
          <li>
            <span className="pagination-ellipsis">&hellip;</span>
          </li>
        )}

        {pagesToDisplay
          .filter((p) => p > 0 && p < pages - 1)
          .map((p) => (
            <li>
              <button
                onClick={() => setPage(p)}
                className={
                  page == p ? "pagination-link is-current" : "pagination-link"
                }
              >
                {p + 1}
              </button>
            </li>
          ))}

        {pagesToDisplay[pagesToDisplay.length - 1] < pages - 2 && (
          <li>
            <span className="pagination-ellipsis">&hellip;</span>
          </li>
        )}
        {pages > 1 && (
          <li>
            <button
              onClick={() => setPage(() => pages - 1)}
              className={
                page == pages - 1
                  ? "pagination-link is-current"
                  : "pagination-link"
              }
            >
              {pages}
            </button>
          </li>
        )}
      </ul>
    </nav>
  );
};

export default PagePicker;
