import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set } from "bumbag"
import PageContent from "../../page/Content"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute
} from "../../user/context"
import sequenceColumn from "../../table/column/sequence"
import { AddButton, EditButton } from "./edit"
import { DeleteButton } from "./delete"
import { Code } from "./menu"
import { useDepartmentList } from "./client"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import AlertDialog from "../../dialog/Alert"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable()
    let { pageIndex, pageSize } = state

    let { isLoading, data, error, mutate, deleteCache } = useDepartmentList(
        toApiParams(state)
    )
    let columns = useColumns({ mutate, items: data?.items })

    return (
        <PageContent
            title={<Trans zh="部门管理" en="Departments" />}
            actionBar={
                <Box alignX="right">
                    <AddButton onSubmitted={mutate} />
                </Box>
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const useColumns = ({ mutate, items = [] }) => {
    let hasAnyMenu = useHasSubMenuByRoute()
    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            {
                Header: <Trans zh="部门名称" en="Name" />,
                id: "name",
                accessor: "name",
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                }
            }
        ]

        if (hasAnyMenu) {
            columns.push({
                id: "actions",
                disableSortBy: true,
                accessor: (row) => row,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    let { id, name } = value
                    return (
                        <Box alignX="center">
                            {
                                <Set>
                                    <EditButton
                                        id={id}
                                        record={value}
                                        onSubmitted={mutate}
                                    />
                                    <DeleteButton
                                        id={id}
                                        name={name}
                                        onSubmitted={mutate}
                                    />
                                </Set>
                            }
                        </Box>
                    )
                }
            })
        }
        return columns
    }, [JSON.stringify(items)])
}
