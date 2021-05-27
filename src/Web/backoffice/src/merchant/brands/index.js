import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Box, Set, Button, Text, Switch, Stack } from "bumbag"
import PageContent from "../../page/Content"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute
} from "../../user/context"
import { Code, Update as UpdateMenu } from "./menu"
import { Trans, useTrans } from "../../i18n"
import { Table, toApiParams, useTable, pageCount } from "../../table"
import {
    useBrandList,
    useChangeBrandStatus,
    useSyncBrandGameList
} from "./client"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import { AddButton, EditButton, ViewButton } from "./edit"
import { SearchButton } from "../../button/SearchButton"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { InputField } from "../../input/Input"
import equals from "fast-deep-equal"
import { MerchantStatusMenu } from "../status"
import { EditBrandBetLimitGameProviderBetLimit } from "./BrandBetLimitGameProviderBetLimitDialog"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"

export { Code }

export const Brand = "Brand"

export const BrandTrans = {
    zh: "品牌",
    en: "Brand"
}

export const useBrandLabel = () => useTrans(BrandTrans)

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        keyword: ""
    })
    let { pageIndex, pageSize, keyword } = state

    let { isLoading, data, error, mutate, deleteCache } = useBrandList(
        toApiParams(state)
    )

    let columns = useColumns({ items: data?.items, mutate })

    return (
        <PageContent
            title={<Trans zh="品牌列表" en="Brands" />}
            actionBar={
                <>
                    <SearchForm
                        search={(data = {}) => {
                            setState({
                                pageIndex: 0,
                                ...data
                            })
                            mutate()
                        }}
                        isLoading={isLoading}
                    />
                    <AddButton onSubmitted={mutate} />
                </>
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

const SearchForm = ({ search, isLoading, ...props }) => {
    let formMethods = useForm({
        defaultValues: {
            keyword: ""
        }
    })

    let searchPlaceHolder = useTrans({
        zh: "关键字输入框",
        en: "Search"
    })

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <InputField
                        name="keyword"
                        showEmptyFieldValueAddonAfter
                        placeholder={searchPlaceHolder}
                    />

                    <SearchButton isLoading={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [], mutate }) => {
    let hasAnyMenu = useHasSubMenuByRoute()

    return useMemo(() => {
        let columns = [
            sequenceColumn(),
            textColumn({
                accessor: "licenseeName",
                Header: <Trans zh="所属公司" en="Licensee Name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "code",
                Header: <Trans zh="品牌代码" en="Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="品牌名称" en="Name" />,
                disableSortBy: true
            }),
            {
                accessor: "description",
                Header: <Trans zh="品牌简介" en="Description" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                }
            },
            {
                accessor: "operationalEmail",
                Header: <Trans zh="品牌邮箱" en="Email" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                }
            },
            textColumn({
                id: "status",
                accessor: (record) => record,
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    return (
                        <Box alignX="center">
                            <MerchantStatusMenu
                                record={value}
                                useChangeStatus={useChangeBrandStatus}
                                onSubmitted={mutate}
                            />
                        </Box>
                    )
                }
            }),
            dateTimeColumn({
                Header: <Trans zh="创建时间" en="Created Time" />,
                accessor: "createdOn",
                disableSortBy: true
            }),
            dateTimeColumn({
                Header: <Trans zh="更改时间" en="Updated Time" />,
                accessor: "updatedOn",
                disableSortBy: true
            })
        ]

        if (hasAnyMenu) {
            columns.push(
                textColumn({
                    id: "actions",
                    disableSortBy: true,
                    Header: <Trans zh="操作" en="Actions" />,
                    accessor: (row) => row,
                    Cell: ({ value }) => {
                        return (
                            <Set orientation="horizontal" alignX="center">
                                <ViewButton record={value} />
                                <EditButton
                                    record={value}
                                    onSubmitted={mutate}
                                />
                                <EditBrandBetLimitGameProviderBetLimit
                                    record={value}
                                    onSubmitted={mutate}
                                />
                            </Set>
                        )
                    }
                })
            )
        }
        return columns
    }, [JSON.stringify(items)])
}
