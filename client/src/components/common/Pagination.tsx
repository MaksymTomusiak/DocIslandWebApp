import { Button, Stack, Typography } from '@mui/material';

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
    className?: string;
}

export function Pagination({
    currentPage,
    totalPages,
    onPageChange,
    className = '',
}: PaginationProps) {
    return (
        <Stack
            direction="row"
            spacing={2}
            alignItems="center"
            className={className}
        >
            <Button
                variant="outlined"
                onClick={() => onPageChange(currentPage - 1)}
                disabled={currentPage === 1}
                size="small"
            >
                Previous
            </Button>
            <Typography color="text.secondary" fontFamily="secondary-regular">
                Page {currentPage} of {totalPages}
            </Typography>
            <Button
                variant="outlined"
                onClick={() => onPageChange(currentPage + 1)}
                disabled={currentPage === totalPages}
                size="small"
            >
                Next
            </Button>
        </Stack>
    );
}
