// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc
{
    public class ControllerActionInvokerProvider : IActionInvokerProvider
    {
        private readonly IControllerActionArgumentBinder _argumentBinder;
        private readonly IControllerFactory _controllerFactory;
        private readonly INestedProviderManager<FilterProviderContext> _filterProvider;
        private readonly IInputFormattersProvider _inputFormatterProvider;
        private readonly IInputFormatterSelector _inputFormatterSelector;
        private readonly IModelBinderProvider _modelBinderProvider;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IModelValidatorProviderProvider _modelValidationProviderProvider;
        private readonly IValueProviderFactoryProvider _valueProviderFactoryProvider;

        public ControllerActionInvokerProvider(
            IControllerFactory controllerFactory,
            IInputFormattersProvider inputFormatterProvider,
            INestedProviderManager<FilterProviderContext> filterProvider,
            IControllerActionArgumentBinder argumentBinder,
            IModelMetadataProvider modelMetadataProvider,
            IInputFormatterSelector inputFormatterSelector,
            IModelBinderProvider modelBinderProvider,
            IModelValidatorProviderProvider modelValidationProviderProvider,
            IValueProviderFactoryProvider valueProviderFactoryProvider)
        {
            _controllerFactory = controllerFactory;
            _inputFormatterProvider = inputFormatterProvider;
            _filterProvider = filterProvider;
            _argumentBinder = argumentBinder;
            _modelMetadataProvider = modelMetadataProvider;
            _modelBinderProvider = modelBinderProvider;
            _inputFormatterSelector = inputFormatterSelector;
            _modelValidationProviderProvider = modelValidationProviderProvider;
            _valueProviderFactoryProvider = valueProviderFactoryProvider;
        }

        public int Order
        {
            get { return DefaultOrder.DefaultFrameworkSortOrder; }
        }

        public void Invoke(ActionInvokerProviderContext context, Action callNext)
        {
            var actionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;

            if (actionDescriptor != null)
            {
                context.Result = new ControllerActionInvoker(
                                    context.ActionContext,
                                    _filterProvider,
                                    _controllerFactory,
                                    actionDescriptor,
                                    _modelMetadataProvider,
                                    _inputFormatterProvider,
                                    _inputFormatterSelector,
                                    _argumentBinder,
                                    _modelBinderProvider,
                                    _modelValidationProviderProvider,
                                    _valueProviderFactoryProvider);
            }

            callNext();
        }
    }
}
