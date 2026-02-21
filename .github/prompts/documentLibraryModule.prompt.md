---
name: documentLibraryModule
---
# Document a Complete Library Module

**Обязательное требование: Вся коммуникация ведётся на русском языке. Все комментарии в коде должны быть на русском языке. Все сообщения исключений (Exception messages) должны быть на русском языке.**

Create comprehensive documentation for a collection of related classes in a folder/module. This includes:

1. **Identify the module structure**
   - List all public types (classes, interfaces, structs) in the specified folder/module
   - Understand the relationships between types
   - Determine the primary use cases

2. **Add quality XML documentation to all public types**
   - Add `<summary>` description for the type itself
   - Add `<remarks>` section explaining purpose, usage patterns, and specialization
   - Document all public properties with `<summary>` and `<value>` tags
   - Document all public methods with:
     - `<summary>` describing what the method does
     - `<param>` tags for all parameters
     - `<returns>` tag describing the return value
     - `<exception>` tags for exceptions that can be thrown
     - `<example>` blocks containing realistic code samples in `<![CDATA[...]]>` blocks
   - Follow the tag order: `<summary>` → `<param>` → `<returns>` → `<exception>` → `<remarks>` → `<example>`

3. **Create or update README.MD in the module folder** with:
   - Brief description of the module's purpose and algorithm/concept
   - List of all public classes with their roles
   - For each class:
     - Constructor signature and description
     - All public properties/methods with signatures
     - Practical usage examples
     - Recommendations for when to use each class/method
   - General guidelines for parameter tuning
   - Performance considerations
   - Common exceptions and error handling
   - Usage tips and best practices

4. **Ensure documentation consistency**
   - All examples should be copy-paste ready
   - Use the same code style throughout
   - Follow the repository's documentation conventions
   - Verify that all examples are technically accurate

5. **Verify compilation**
   - Run a build to ensure XML comments don't break compilation
   - Confirm all changes are syntactically correct

**Goal**: Developers should be able to understand the entire module from the XML docs and README, with clear examples they can copy and modify for their use case.
